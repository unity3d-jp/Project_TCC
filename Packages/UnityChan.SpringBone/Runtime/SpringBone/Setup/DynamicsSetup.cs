using UTJ.StringQueueExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public class DynamicsSetup
    {
        // Public interface

        // Reads a SpringBone configuration from CSV source text and builds it if possible.
        // Any errors will be logged. Returns true if something was able to be built, false otherwise.
        public static bool BuildFromRecordText
        (
            GameObject springBoneRoot,
            GameObject colliderRoot,
            string recordText,
            ImportSettings importSettings = null,
            IEnumerable<string> requiredBones = null
        )
        {
            var parsedSetup = ParseFromRecordText(springBoneRoot, colliderRoot, recordText, importSettings);
            var canBuild = parsedSetup.Setup != null;
            if (canBuild)
            {
                parsedSetup.Setup.Build(requiredBones);
            }
            return canBuild;
        }

        // Reads a SpringBone configuration from CSV source text.
        // If the Setup field in the ParseResults is not null, it can be built from.
        // Any errors will be reported in the Errors field of the ParseResults.
        public static ParseResults ParseFromRecordText
        (
            GameObject springBoneRoot,
            GameObject colliderRoot,
            string recordText,
            ImportSettings importSettings = null
        )
        {
            if (springBoneRoot == null) { throw new System.ArgumentNullException("springBoneRoot"); }
            if (colliderRoot == null) { throw new System.ArgumentNullException("colliderRoot"); }
            if (recordText == null) { throw new System.ArgumentNullException("recordText"); }

            var results = InternalParseFromRecordText(springBoneRoot, colliderRoot, recordText, importSettings);
            results.LogErrors();
            return results;
        }

        // If requiredBoneList is not null, bones not in the list will not be created,
        // and bones in the list not listed in the CSV will be created with default parameters.
        public void Build(IEnumerable<string> requiredBones = null)
        {
            if (importSettings.ImportCollision && colliderSetup != null)
            {
                colliderSetup.BuildObjects(colliderRoot);
            }

            if (importSettings.ImportSpringBones && springBoneSetup != null)
            {
                springBoneSetup.BuildObjects(springBoneRoot, colliderRoot, requiredBones);
            }
        }

        // Utility class

        public class ImportSettings
        {
            public ImportSettings()
            {
                ImportSpringBones = true;
                ImportCollision = true;
            }

            public ImportSettings(ImportSettings sourceSettings)
            {
                ImportSpringBones = sourceSettings.ImportSpringBones;
                ImportCollision = sourceSettings.ImportCollision;
            }

            public bool ImportSpringBones { get; set; }
            public bool ImportCollision { get; set; }
        }

        public class ParseMessage
        {
            public ParseMessage(string message, string sourceLine = "", string exception = "", Object context = null)
            {
                Message = message;
                SourceLine = sourceLine;
                Exception = exception;
                Context = context;
            }

            public ParseMessage(string message, IEnumerable<string> sourceLineItems, string exception = "", Object context = null)
            {
                Message = message;
                SourceLine = (sourceLineItems != null) ? string.Join(", ", sourceLineItems.ToArray()) + "\n" : "";
                Exception = exception;
                Context = context;
            }

            public string Message { get; private set; }
            public string SourceLine { get; private set; }
            public string Exception { get; private set; }
            public Object Context { get; private set; }

            public string ToLogMessage()
            {
                var message = Message;
                if (!string.IsNullOrEmpty(SourceLine)) { message += "\n" + SourceLine; }
                if (!string.IsNullOrEmpty(Exception)) { message += "\n\n" + Exception; }
                return message;
            }
        }

        public class ParseResults
        {
            public ParseResults()
            {
                Setup = null;
                Errors = new List<ParseMessage>(0);
            }

            public ParseResults(DynamicsSetup setup, IEnumerable<ParseMessage> errors)
            {
                Setup = setup;
                Errors = errors.ToList();
            }

            public static ParseResults Failure(string error)
            {
                return new ParseResults(null, new List<ParseMessage> { new ParseMessage(error) });
            }

            public static ParseResults Failure(IEnumerable<ParseMessage> errors)
            {
                return new ParseResults(null, errors);
            }

            public DynamicsSetup Setup { get; private set; }
            public List<ParseMessage> Errors { get; private set; }
            public bool HasErrors { get { return Errors.Count > 0; } }

            public void LogErrors()
            {
                foreach (var error in Errors)
                {
                    Debug.LogError(error.ToLogMessage(), error.Context);
                }
            }
        }

        // For internal use

        public static int GetVersionFromSetupRecords
        (
            List<TextRecordParsing.Record> sourceRecords,
            out TextRecordParsing.Record versionRecord
        )
        {
            var version = UnknownVersion;
            const string VersionToken = "version";
            versionRecord = sourceRecords.FirstOrDefault(item => item.GetString(0).ToLowerInvariant() == VersionToken);
            if (versionRecord != null) { versionRecord.TryGetInt(1, ref version); }
            return version;
        }

        public static System.Object SerializeObjectFromStrings
        (
            System.Type type,
            IEnumerable<string> sourceItems,
            string firstOptionalField,
            ref ParseMessage error
        )
        {
            try
            {
                var sourceQueue = new Queue<string>(sourceItems);
                return sourceQueue.DequeueObject(type, firstOptionalField);
            }
            catch (System.Exception exception)
            {
                error = new ParseMessage("Error building " + type.ToString(), sourceItems, exception.ToString());
            }
            return null;
        }

        public static T SerializeObjectFromStrings<T>
        (
            IEnumerable<string> sourceItems,
            string firstOptionalField,
            ref ParseMessage error
        ) where T : class
        {
            return SerializeObjectFromStrings(typeof(T), sourceItems, firstOptionalField, ref error) as T;
        }
        
        // private

        private const int UnknownVersion = -1;

        private ImportSettings importSettings;
        private GameObject springBoneRoot;
        private GameObject colliderRoot;
        private SpringBoneSerialization.ParsedSpringBoneSetup springBoneSetup;
        private SpringColliderSerialization.ParsedColliderSetup colliderSetup;

        private static int GetVersionFromSetupRecords(List<TextRecordParsing.Record> sourceRecords)
        {
            TextRecordParsing.Record unusedRecord;
            return GetVersionFromSetupRecords(sourceRecords, out unusedRecord);
        }

        // Version and CSV content detection
        // If version 3, import settings will be changed to reflect whether the file is bone-only or collider-only
        private static bool VerifyVersionAndDetectContents(string recordText, ImportSettings importSettings, out string errorMessage)
        {
            errorMessage = "";
            var version = UnknownVersion;
            try
            {
                var sourceRecords = TextRecordParsing.ParseRecordsFromText(recordText);
                version = GetVersionFromSetupRecords(sourceRecords);
            }
            catch (System.Exception exception)
            {
                errorMessage = string.Format("SpringBoneSetup: ���̃e�L�X�g�f�[�^��ǂݍ��߂܂���ł����I\n\n{0}", exception.ToString());
                return false;
            }

            const int VersionSpringBonesOnly = 3;
            const int MinSupportedVersion = VersionSpringBonesOnly;
            const int MaxSupportedVersion = 4;

            if (version == UnknownVersion)
            {
                // No version means it's probably colliders-only, but check if there are SpringBones just in case
                if (!recordText.ToLowerInvariant().Contains("[springbones]"))
                {
                    importSettings.ImportSpringBones = false;
                }
            }
            else
            {
                if (version < MinSupportedVersion
                    || version > MaxSupportedVersion)
                {
                    errorMessage = string.Format("SpringBoneSetup: �f�[�^�̃o�[�W�����͑Ή����Ă��܂���I\nVersion: {0}", version);
                    return false;
                }

                if (version <= VersionSpringBonesOnly)
                {
                    importSettings.ImportCollision = false;
                }
            }

            return true;
        }

        private static ParseResults InternalParseFromRecordText
        (
            GameObject springBoneRoot,
            GameObject colliderRoot,
            string recordText,
            ImportSettings importSettings
        )
        {
            if (recordText.Length == 0) { return new ParseResults(); }

            // Copy the source import settings in case we need to change them based on the source text
            var actualImportSettings = (importSettings != null)
                ? new ImportSettings(importSettings)
                : new ImportSettings();

            string errorMessage;
            if (!VerifyVersionAndDetectContents(recordText, actualImportSettings, out errorMessage))
            {
                return ParseResults.Failure(errorMessage);
            }

            var errors = new List<ParseMessage>();

            SpringColliderSerialization.ParsedColliderSetup colliderSetup = null;
            if (actualImportSettings.ImportCollision)
            {
                colliderSetup = SpringColliderSerialization.ParsedColliderSetup.ReadColliderSetupFromText(
                    colliderRoot, recordText);
                if (colliderSetup == null)
                {
                    errors.Add(new ParseMessage("�_�C�i�~�N�X�Z�b�g�A�b�v�����s���܂����F���f�[�^�ɃG���[������܂�"));
                    return ParseResults.Failure(errors);
                }
                else
                {
                    errors.AddRange(colliderSetup.Errors);
                }
            }

            SpringBoneSerialization.ParsedSpringBoneSetup springBoneSetup = null;
            if (actualImportSettings.ImportSpringBones)
            {
                var validColliderNames = (colliderSetup != null) ? colliderSetup.GetColliderNames() : null;
                springBoneSetup = SpringBoneSerialization.ParsedSpringBoneSetup.ReadSpringBoneSetupFromText(
                    springBoneRoot, colliderRoot, recordText, validColliderNames);
                if (springBoneSetup == null)
                {
                    errors.Add(new ParseMessage("�_�C�i�~�N�X�Z�b�g�A�b�v�����s���܂����F���f�[�^�ɃG���[������܂�"));
                    return ParseResults.Failure(errors);
                }
                else
                {
                    errors.AddRange(springBoneSetup.Errors);
                }
            }

            var dynamicsSetup = new DynamicsSetup
            {
                importSettings = actualImportSettings,
                springBoneRoot = springBoneRoot,
                colliderRoot = colliderRoot,
                springBoneSetup = springBoneSetup,
                colliderSetup = colliderSetup
            };

            return new ParseResults(dynamicsSetup, errors);
        }
    }
}