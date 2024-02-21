# 独自のコントロールの作成

TCCでは、以下のインターフェースを継承することで機能を追加します。

|インターフェース|内容|
|---|---|
|IMove | キャラクターの移動ベクトルを指定する。 |
|ITurn | キャラクターの向きと更新速度を指定する。 |
|IPriorityLifecycle<IMove> | IMoveのプライオリティが高い時の動作。 |
|IPriorityLifecycle<ITurn> | ITurnのプライオリティが高い時の動作。 |
|IEarlyUpdateComponent | Brainのタイミングに依存するコンポーネントの更新処理。<br>ExecuteOrder=0のUpdateより前に実行する。<br>Brainが停止中は動作を停止する。 |
|IUpdateComponent| Brainのタイミングに依存するコンポーネントの更新処理<br>ExecuteOrder=0のUpdateより後に実行する。<br>Brainが停止中は動作を停止する。|


## キャラクターの移動

ここではプライオリティを利用したキャラクターの移動制御を学びます。

キャラクターの移動を実現するControlを作成します。このControlでは、WASDでキャラクターは平行移動し、QEで上下に移動します


まず `MyControl` クラスを作成します。

```cs
public class MyControl : MonoBehaviour
{
    public float Speed = 4;

    public void MoveHorizontal(Vector2 input)
    {
    }

    public void MoveVertical(float input)
    {
    }
}
```

キャラクターの移動処理を実装するため `IMove` を継承します。このインターフェースでは移動のプライオリティと、プライオリティが最も高かった時に使用する移動ベクトルを設定します。

`MoveHorizontal()` と `MoveVertical()` で入力内容を受け取り、`MoveVelocity` の計算に使用します。またカメラの入力方向を使用するため、`CharacterSettings.CameraYawRotation` でキャラクターの移動ベクトルをスクリーンを考慮した方向へ補正します。

このコンポーネントを設定することで、 `Priority` が他のコンポーネントより高い間は入力に沿って移動します。

```cs
public class MyControl : MonoBehaviour, IMove
{
    public float Speed = 4;
    
    public int Priority = 0;

    private Vector3 _inputVelocity;
    private CharacterSettings _settings;

    private void Awake()
    {
        TryGetComponent(out _settings);
    }

    // 平行移動の入力
    public void MoveHorizontal(Vector2 input)
    {
        _inputVelocity = new Vector3(input.x, _inputVelocity.y, input.y);
    }

    // 垂直移動の入力
    public void MoveVertical(float input)
    {
        _inputVelocity.y = input;
    }

    
    int IPriority<IMove>.Priority => Priority;

    Vector3 IMove.MoveVelocity => _settings.CameraYawRotation * _inputVelocity * Speed;
}
```

次にこのコンポーネントが有効の間は重力を無視するようにします。具体的には`IPriorityLifecycle<IMove>`を継承し、コンポーネントがプライオリティで最高値になった際、あるいは最高値を失った際のコールバックを受け取ります。

 `Gravity` コンポーネントを取得し、`OnAcquireHighestPriority`のタイミングでGravityを無効、`OnLoseHighestPriority`のタイミングで失効したGravityを再開します。

```cs
public class MyControl : MonoBehaviour, IMove, IPriorityLifecycle<IMove>
{
    public float Speed = 4;    
    public int Priority = 0;
    private Vector3 _inputVelocity;

    private Gravity _gravity;// <- new

    private CharacterSettings _settings;

    private void Awake()
    {
        TryGetComponent(out _settings);
        TryGetComponent(out _gravity); // <- new
    }

    public void MoveHorizontal(Vector2 input)
    {
        _inputVelocity = new Vector3(input.x, _inputVelocity.y, input.y);
    }

    public void MoveVertical(float input)
    {
        _inputVelocity.y = input;
    }
    
    int IPriority<IMove>.Priority => Priority;

    Vector3 IMove.MoveVelocity => _settings.CameraYawRotation * _inputVelocity * Speed;
    
    
    void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime) { }

    void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
    {
        _gravity.enabled = false;
    }

    void IPriorityLifecycle<IMove>.OnLoseHighestPriority()
    {
        _gravity.enabled = true;
    }
}
```

## キャラクターのダッシュ

ここではプライオリティを利用した向きの更新や、プライオリティ自体の更新について学びます。

キャラクターのダッシュの動きを実現するControlを作成します。これは他のControlの影響を受けず、一定方向へ高速移動を行うControlです。

`Input(...)`でダッシュするべき移動方向を設定し、`Dash()` で現在のキャラクターの入力方向へダッシュします。

```cs
public class DashControl : MonoBehaviour
{
    [Header("Settings")]
    public float DashSpeed = 14;        // ダッシュの移動速度
    public float DashDuration = 0.5f;   // ダッシュの移動時間
    
    public void Input(Vector2 input)
    {
    }

    public void Dash()
    {
    }
}
```

ダッシュの処理を実装します。 `IMove` を継承してプライオリティが最も高い時のみ移動する処理を実装しますが、そのプライオリティはダッシュ中のみ有効になるようにします。

また ` IUpdateComponent` を継承し、` IUpdateComponent.OnUpdate(...)` でダッシュの速度を減速します。このUpdateはUnity標準のUpdateと異なり、Brainの実行タイミングによって FixedUpdate もしくは Update で動作します。
この`IUpdateComponent`の実行順は `IUpdateComponent.Order` で指定します。例えば他の `IUpdateComponent` の実行結果を使用したい場合、Orderを増やしてタイミングを制御します。


```cs
public class DashControl : MonoBehaviour, IMove, IUpdateComponent
{
    [Header("Priority")]
    public int MovePriority = 0;

    [Header("Settings")]
    public float DashSpeed = 14;
    public float DashDuration = 0.5f;
    
    private IWallCheck _wallCheck; // 壁との接触判定
    private CharacterSettings _settings;

    private Vector3 _direction; // ダッシュの方向
    private float _currentSpeed; // 現在のダッシュの速度
    private float _dashEndTime; // ダッシュが終了する時間
    private Vector2 _input; // 入力内容のキャッシュ

    private void Awake()
    {
        TryGetComponent(out _wallCheck);
        TryGetComponent(out _settings);
    }

    public void Input(Vector2 input)
    {
        _input = input;
    }

    public void Dash()
    {
        // 移動方向が指定されていなければダッシュしない
        if (_input.sqrMagnitude < 0.5f)
            return;

        // ダッシュの方向を設定
        _direction = _settings.CameraYawRotation * new Vector3(_input.x, 0, _input.y);

        // ダッシュの速度を設定
        _currentSpeed = DashSpeed;
        _dashEndTime = Time.time + DashDuration;
    }

    // ダッシュが有効な間のみプライオリティを有効にする
    int IPriority<IMove>.Priority => _dashEndTime > Time.time ? MovePriority : 0;

    Vector3 IMove.MoveVelocity => _direction * _currentSpeed;
    
    void IUpdateComponent.OnUpdate(float deltaTime)
    {
        // ダッシュ中のみ処理する
        if (_dashEndTime < Time.time)
            return;

        // 壁に当たったら停止する
        if (_wallCheck.IsContact)
            Stop();
        
        // DashDurationのタイミングで減速が完了するように速度を遅くする。
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, deltaTime * DashSpeed / DashDuration);
    }

    int IUpdateComponent.Order => Order.Control;

    private void Stop()
    {
        _currentSpeed = 0;
        _dashEndTime = 0;
    }
}
```

最後にダッシュ中はキャラクターの向きダッシュ方向へ向くようにします。

キャラクターの向きは `ITurn` で設定します。 `IMove` と同様にプライオリティで動作し、`YawAngle` でキャラクターが向くべき方向を、`TurnSpeed` でキャラクターの向きの更新速度を指定します。なお `TurnSpeed` が -1 の場合、キャラクターは即座に指定の方向を向きます。


```cs
public class DashControl : MonoBehaviour, IMove, IUpdateComponent, ITurn
{
    [Header("Priority")]
    public int MovePriority = 0;
    public int TurnPriority = 0;

    [Header("Settings")]
    [Range(-1, 50)]
    public int TurnSpeed = -1;
    public float DashSpeed = 14;
    public float DashDuration = 0.5f;
    
    private Vector3 _direction;
    private IWallCheck _wallCheck;
    private CharacterSettings _settings;

    private float _currentSpeed;
    private float _dashEndTime;
    private float _yawAngle;
    private Vector2 _input;

    private void Awake()
    {
        TryGetComponent(out _wallCheck);
        TryGetComponent(out _settings);
    }

    public void Input(Vector2 input)
    {
        _input = input;
    }

    public void Dash()
    {
        if (_input.sqrMagnitude < 0.5f)
            return;

        _direction = _settings.CameraYawRotation * new Vector3(_input.x, 0, _input.y);
        _currentSpeed = DashSpeed;
        _dashEndTime = Time.time + DashDuration;
        _yawAngle = Vector3.SignedAngle(Vector3.forward, _direction, Vector3.up);
    }

    int IPriority<IMove>.Priority => _dashEndTime > Time.time ? MovePriority : 0;

    Vector3 IMove.MoveVelocity => _direction * _currentSpeed;
    
    void IUpdateComponent.OnUpdate(float deltaTime)
    {
        if (_dashEndTime < Time.time)
            return;
        
        if (_wallCheck.IsContact)
            Stop();
        
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, deltaTime * DashSpeed / DashDuration);
    }

    private void Stop()
    {
        _currentSpeed = 0;
        _dashEndTime = 0;
    }

    int IUpdateComponent.Order => Order.Control;

    int IPriority<ITurn>.Priority => _dashEndTime > Time.time ? TurnPriority : 0;

    int ITurn.TurnSpeed => TurnSpeed;

    float ITurn.YawAngle => _yawAngle;
}
```

## 指定座標への移動

他のEffectやControlの影響を受けない `IWarp` による移動について紹介します。
`IWarp` は特定の座標へキッチリと到達したい場合に有効です。また `IWarp.Warp(...)` では障害物を考慮しませんが、 `IWarp.Move(...)` では移動の中間点を考慮します。考慮内容はBrainに依存します。
`IWarp` で移動した場合、そこで指定した座標は他のControlやEffectより優先されます。またEffectの内容をリセットします。

`ITransform`はTransformの内容をキャッシュし使用するコンポーネントです。フレームの最初にTransformの内容をキャッシュし、その値を使用します。また値を設定した場合、即座に反映します。

以下は二点の座標を一定時間で往復するコンポーネントです。

```cs
public class WarpControl : MonoBehaviour, IMove, IPriorityLifecycle<IMove>
{
    public int Priority = 15;
    public float Duration = 5;
    public Transform Start, End;

    private IWarp _warp;
    private ITransform _transform;
    private float _timeAmount = 0;
    private Vector3 _velocity;

    private void Awake()
    {
        TryGetComponent(out _warp);
        TryGetComponent(out _transform);
    }

    int IPriority<IMove>.Priority => Priority;

    Vector3 IMove.MoveVelocity => _velocity;

    void IPriorityLifecycle<IMove>.OnUpdateWithHighestPriority(float deltaTime)
    {
        _timeAmount += deltaTime /  Duration;
        _timeAmount %= 1;
        
        var pos = Vector3.Lerp(Start.position, End.position, _timeAmount);

        _velocity = (_transform.Position - pos) / deltaTime;
        _warp.Move(pos);
    }

    void IPriorityLifecycle<IMove>.OnAcquireHighestPriority()
    {
        _timeAmount = 0;
    }

    void IPriorityLifecycle<IMove>.OnLoseHighestPriority() { }
}

```