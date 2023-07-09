//
//    `````              Code founded by Alex Mankin                       
// .:///////:-.                               -.    
//./////////////:-`          `....``    ```.-://:-` 
//`/////////////////:.``  .:///////////////////+yy/-
//  `..---:://////////////////////////////////::::.`
//           `-://////////////////////////////`     
//                ```````.//////::::--..-////.      
//                      -//////-       `--``-/`     
//                      -//:.`         .     ./.    
//                        -::.                ``  
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class ImportTextFromMFS {

    //VARIABLES
    private static string line, colonSplitLine, currentGroup;
    private static string[] colonDividedLines;
    private static AllPersons currentPerson = AllPersons._NULL;
    private static TextData currentTextData;
    private static Locale currentLocale;
    private static TextMessage currentMessage;
    private static List<string> enumSequenceNames, enumGroupNames;

    //CONSTANTS
    public const string TEXT_PATH = "Assets/Import/Text/",
                        RESOURCE_PATH = "Assets/Resources/Text/",
                        PERSONS_PATH = "Assets/Objects/Persons/",
                        SCENE_PATH = "Assets/_Scenes/",
                        FOLDER_TEXT = "Text/",
                        ENUM_NAME_TEXT_GROUPS = "TextGroups";

    //EVENTS

    //METHODS
    [MenuItem("Mystery Teens/Import Text (MFS)")]
    public static void Import() {

        DirectoryInfo dir;
        FileInfo[] fileInfo;

        enumGroupNames = new List<string>();

        dir = new DirectoryInfo(TEXT_PATH);
        fileInfo = dir.GetFiles("*" + Global.FILE_EXT_MFS);

        for (int i = 0; i < fileInfo.Length; ++i) {
            ImportFile(fileInfo[i].FullName);
        }

        EnumBuilder.Build(ENUM_NAME_TEXT_GROUPS, enumGroupNames.ToArray());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //Delete unused
    }

    public static void ImportFile(string sourcePath) {

        currentLocale = Locale.English;
        currentGroup = Path.GetFileNameWithoutExtension(sourcePath);
        bool useResourcePath = false;

        enumSequenceNames = new List<string>();

        using (StreamReader sr = new StreamReader(sourcePath)) {
            Debug.Log(sourcePath);
            string currentSequence = "", currentTargetDirectory = "";
            bool bracketSkip = false;
            currentTextData = null;

            int currentLine = 0;

            while ((line = sr.ReadLine()) != null) {
                currentLine++;
                line = line.Replace('\t', ' ');
                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line[0] == ';')
                    continue;

                if (line[0] == '/') {
                    if (line[1] == '/') {
                        bracketSkip = !bracketSkip;
                        continue;
                    }
                }

                if (bracketSkip) {
                    continue;
                }

                if (line[0] == '?') {
                    SaveTextData(currentTextData, currentTargetDirectory, currentSequence);

                    currentSequence = line.Substring(1);
                    enumSequenceNames.Add(currentSequence);
                    string filePath = currentTargetDirectory + currentSequence + Global.FILE_EXT_ASSET;

                    if (File.Exists(filePath)) {
                        currentTextData = AssetDatabase.LoadAssetAtPath<TextData>(filePath);
                        //TODO: Merge later. for now we'll just make new messages from scratch.
                        currentTextData.Messages.Clear();
                    }
                    else
                        currentTextData = ScriptableObject.CreateInstance<TextData>();

                    currentTextData.SequenceName = currentSequence;
                    currentTextData.GroupName = currentGroup;
                    continue;
                }

                if (line[0] == '@') {
                    SaveTextData(currentTextData, currentTargetDirectory, currentSequence);
                    currentTextData = null;

                    if (line[1] == 'd') {
                        useResourcePath = true;
                        enumGroupNames.Add(currentGroup);
                        currentTargetDirectory = RESOURCE_PATH + currentGroup + '/';
                    }
                    else {
                        if (useResourcePath)
                            EnumBuilder.Build(currentGroup, enumSequenceNames.ToArray());

                        useResourcePath = false;
                        string sceneCode = line.Split(' ')[0].Substring(1);

                        currentTargetDirectory = SCENE_PATH + sceneCode + '/';
                        if (string.IsNullOrEmpty(currentTargetDirectory)) {
                            break;
                        }

                        currentTargetDirectory += FOLDER_TEXT;
                    }
                    

                    Debug.Log("TARGET DIRECTORY: " + currentTargetDirectory);
                    continue;
                }

                if (currentTextData == null) {
                    Debug.LogWarningFormat("Cannot parse text, no sequence (? operator) specified.\n{0} [{1}]", currentGroup, currentLine);
                    continue;
                }

                if (line.Contains(": ")) {
                    colonDividedLines = line.Split(':');
                    Enum.TryParse(colonDividedLines[0], true, out currentPerson);

                    line = line.Substring(line.IndexOf(": ") + 2);
                }
                line = line.Trim();

                //If we made it here, we're adding a new message!
                currentMessage = new TextMessage();
                currentMessage.iHaveData = true;
                currentMessage.textData = currentTextData;
                currentMessage.person = currentPerson;

                string personDataPath = PERSONS_PATH + currentPerson.ToString() + Global.FILE_EXT_ASSET;
                if (File.Exists(personDataPath))
                    currentMessage.personData = AssetDatabase.LoadAssetAtPath<PersonData>(personDataPath);
  

                string finalString = "";
                string[] lineSpaceSplit = line.Split(' ');

                for (int i = 0; i < lineSpaceSplit.Length; ++i) {
                    if (string.IsNullOrEmpty(lineSpaceSplit[i]))
                        continue;

                    if (lineSpaceSplit[i][0] == '#') {
                        string currentHashtag = lineSpaceSplit[i].Substring(1);

                        if (currentHashtag.Equals("notext")) {
                            currentMessage.noText = true;
                        }
                        else if (currentHashtag.Equals("skip")) {
                            currentMessage.autoSkip = true;
                        }
                        else if (currentHashtag.Equals("left")) {
                            currentMessage.forceLeft = true;
                        }
                        else if (currentHashtag.Equals("right")) {
                            currentMessage.forceRight = true;
                        }
                        else if (currentHashtag.Equals("bump")) {
                            currentMessage.bumpExisting = true;
                        }

                    }
                    else if (lineSpaceSplit[i][0] == '^') {
                        int.TryParse(lineSpaceSplit[i].Substring(1), out currentMessage.portraitNo);
                    }
                    else {
                        finalString += lineSpaceSplit[i] + ' ';
                    }
                }

                if (string.IsNullOrEmpty(finalString) ? false : finalString[0] == '(')
                    currentMessage.innerThoughts = true;

                finalString = finalString.Trim();
                currentMessage.textLocales[(int)currentLocale] = finalString;
                currentMessage.animation = currentMessage.personData?.GetPortrait(currentMessage.portraitNo);
                currentTextData.Messages.Add(currentMessage);
            }
            SaveTextData(currentTextData, currentTargetDirectory, currentSequence);

            if (useResourcePath)
                EnumBuilder.Build(currentGroup, enumSequenceNames.ToArray());

        }

        void SaveTextData(TextData dataToSave, string outputDirectory, string objectName) {
            if (dataToSave == null)
                return;

            if (string.IsNullOrEmpty(outputDirectory) || string.IsNullOrEmpty(objectName))
                return;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            string fullFileName = outputDirectory + objectName + Global.FILE_EXT_ASSET;
            string verb;

            if (File.Exists(fullFileName)) {
                //TODO: Update this to merge the files once we start messing with locales.
                verb = "Updated ";

            }
            else {
                verb = "Created ";
                AssetDatabase.CreateAsset(dataToSave, fullFileName);
            }

            generateHelperFile(currentTextData);
            EditorUtility.SetDirty(dataToSave);
            Debug.Log(verb + objectName);
        }

         void generateHelperFile(TextData data) {
            string output = "";
            foreach (TextMessage M in data.Messages) {
                output += "yield return MessageBox.Display("
                    + data.Messages.IndexOf(M) + "); // "
                    + (M.person != AllPersons._NULL ? M.person.ToString() + ": " : "")
                    + M.textLocales[0] + "\n";
            }
            output += "}";

            data.CodeHelper = output;
        }
    }

    //PROPERTIES
}

