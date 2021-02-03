using MelvorIdleModAssistant.ViewModels;
using ReactiveUI;
using System.Collections.Generic;

namespace MelvorIdleModAssistant {
    class Console {
        public static MainWindowViewModel MainWindowVM;

        private static string lastConsoleLine = "";
        public static string LastConsoleLine
        {
            get
            {
                return lastConsoleLine;
            }
            private set
            {
                lastConsoleLine = value;
                MainWindowVM.OnPropertyChanged(nameof(LastConsoleLine));
            }
        }

        public static void Log(string message) {
            System.Console.WriteLine(message);
            LastConsoleLine = message;
        }

        public static void Log(object obj) {
            Log(obj.ToString());
        }
    }
}
