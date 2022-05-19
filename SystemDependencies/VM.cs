using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SystemDependencies
{
    class VM : INotifyPropertyChanged
    {
        #region Properties

        const string Title_Dialog = "Open Text File";
        const string Filter = "TXT files|*.txt";

        private readonly Dictionary<String, Software> allSoftware = new Dictionary<String, Software>();
        private readonly List<Software> installedSoftwares = new List<Software>();
        private readonly List<Software> explicitlyInstalledSoftwares = new List<Software>();
        private readonly List<string> inputs = new List<string>();

        private string output;
        public string Output
        {
            get { return output; }
            set { output = value; PropChange(); }
        }

        private string manuallyInput;
        public string ManuallyInput
        {
            get { return manuallyInput; }
            set { manuallyInput = value; PropChange(); }
        }
        #endregion

        #region Methods
        public void RunFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = Title_Dialog,
                Filter = Filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.OpenFile()))
                {
                    while (!reader.EndOfStream)
                    {
                        inputs.Add(reader.ReadLine().Trim());
                    }
                }
                Run();
            }
        }

        public void RunManuallyInput()
        {
            if (!(ManuallyInput is null))
            {
                foreach (string input in ManuallyInput.Split('\n'))
                {
                    if (!(input.Trim() == ""))
                        inputs.Add(input.Trim());
                }
                Run();
                ManuallyInput = null;
            }
            else
            {
                Output += "Please input command\n";
            }
        }

        private void Run()
        {
            foreach (String commandLine in inputs)
            {
                String[] commands = Regex.Split(commandLine, "\\s+");
                String singleCommand = commands[0];
                Output +=  commandLine + "\n";
                switch (singleCommand)
                {
                    case "DEPEND":
                        String softwareName = commands[1];
                        BuildDependencies(softwareName, commands);
                        break;
                    case "INSTALL":
                        Software softwareToBeInstalled = GetSoftware(commands[1]);
                        if (IsAlreadyInstalled(softwareToBeInstalled))
                        {
                            Output += "\t" + softwareToBeInstalled.GetName() + " is already installed\n";
                        }
                        else
                        {
                            List<Software> softwareDependenciesToBeInstalled = softwareToBeInstalled.GetDependencies();
                            foreach (Software softwareDependency in softwareDependenciesToBeInstalled)
                            {
                                List<Software> DependenciesToBeInstalled = softwareDependency.GetDependencies();
                                foreach(Software dependency in DependenciesToBeInstalled)
                                {
                                    if (!IsAlreadyInstalled(dependency))
                                    {
                                        Install(dependency);
                                    }
                                }
                                if (!IsAlreadyInstalled(softwareDependency))
                                {
                                    Install(softwareDependency);
                                }
                            }
                            Install(softwareToBeInstalled);
                            explicitlyInstalledSoftwares.Add(softwareToBeInstalled);
                        }
                        break;
                    case "REMOVE":
                        Software softwareToBeRemoved = GetSoftware(commands[1]);
                        if (!IsAlreadyInstalled(softwareToBeRemoved))
                        {
                            Output += "\t" + softwareToBeRemoved.GetName() + " is not installed\n";
                        }
                        else if (CanRemoveSoftware(softwareToBeRemoved))
                        {
                            Output += "\tRemoving " + softwareToBeRemoved.GetName() + "\n";
                            installedSoftwares.Remove(softwareToBeRemoved);
                            List<Software> currentSoftwareDependencies = softwareToBeRemoved.GetDependencies();
                            foreach (Software dependency in currentSoftwareDependencies)
                            {
                                if (CanRemoveSoftware(dependency) && !explicitlyInstalledSoftwares.Contains(dependency))
                                {
                                    Output += "\tRemoving " + dependency.GetName()+"\n";
                                    installedSoftwares.Remove(dependency);
                                }
                                else
                                {
                                    Output += "\t" + dependency.GetName() + " is explicitly installed\n";
                                }
                            }
                            explicitlyInstalledSoftwares.Remove(softwareToBeRemoved);
                        }
                        else
                        {
                            Output += "\t" + softwareToBeRemoved.GetName() + " is still needed\n";
                        }
                        break;
                    case "LIST":
                        foreach (Software installedSoftware in installedSoftwares)
                        {
                            Output += "\t" + installedSoftware.GetName()+"\n";
                        }
                        break;
                    case "END":
                        break;
                    default:
                        Output += " is an invalide command\n";
                        break;
                }
            }
            inputs.Clear();
        }
        private void Install(Software software)
        {
            Output += "\tInstalling " + software.GetName()+"\n";
            installedSoftwares.Add(software);
        }
        private bool IsAlreadyInstalled(Software softwareToBeInstalled)
        {
            return installedSoftwares.Contains(softwareToBeInstalled);
        }
        private void BuildDependencies(String softwareName, String[] commands)
        {
            for (int i = 2; i < commands.Length; i++)
            {
                String currentDependency = commands[i];
                List<Software> depenciesOfDependency = GetSoftware(currentDependency).GetDependencies();
                if (!depenciesOfDependency.Contains(GetSoftware(softwareName)))
                {
                    foreach(Software depency in depenciesOfDependency)
                    {
                        GetSoftware(softwareName).AddDependencies(depency);
                    }
                    GetSoftware(softwareName).AddDependencies(GetSoftware(currentDependency));
                }
            }
        }
        private Software GetSoftware(String name)
        {
            Software software;
            if (!allSoftware.ContainsKey(name))
            {
                software = new Software(name);
                allSoftware.Add(name, software);
            }
            else
            {
                allSoftware.TryGetValue(name, out software);
            }
            return software;
        }
        private bool CanRemoveSoftware(Software softwareToBeRemoved)
        {
            foreach (Software installedSoftware in installedSoftwares)
            {
                List<Software> requiredDependencies = installedSoftware.GetDependencies();
                if (requiredDependencies != null)
                {
                    foreach (Software dependency in requiredDependencies)
                    {
                        if (softwareToBeRemoved.Equals(dependency))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void Clear()
        {
            Output = null;
        }
        #endregion

        #region prop change
        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChange([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
