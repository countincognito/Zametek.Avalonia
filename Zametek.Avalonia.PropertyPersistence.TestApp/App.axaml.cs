using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.IO;

namespace Zametek.Avalonia.PropertyPersistence.TestApp
{
    public partial class App
        : Application
    {
        private readonly string m_PropertyPersistenceFileName;
        private readonly StateResourceAccess m_StateResourceAccess;

        public App()
        {
            m_PropertyPersistenceFileName =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "PropertyPersistence.json");
            m_StateResourceAccess = new StateResourceAccess(m_PropertyPersistenceFileName);
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            string message;
            if (File.Exists(m_PropertyPersistenceFileName))
            {
                message = "I will now attempt to load the following file:\n"
                    + m_PropertyPersistenceFileName + "\n\nThis file will be overwritten when the application closes.";
            }
            else
            {
                message = "I could not find the following file:\n"
                    + m_PropertyPersistenceFileName + "\n\nIt will be created when the application closes.";
            }

            PropertyStateHelper.Load(m_StateResourceAccess);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new Main()
                };

                desktop.Exit += (a, b) =>
                {
                    PropertyStateHelper.Save(m_StateResourceAccess);
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}