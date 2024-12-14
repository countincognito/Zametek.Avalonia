using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Zametek.Avalonia.PropertyPersistence.TestApp
{
    public class Main
        : ReactiveObject
    {
        #region Fields

        private double m_MyHeight = 100.0;
        private double m_HeightIncrement = 20.0;
        private int m_MyTabIndex = 0;
        private ICommand m_HeightIncrementCommand;
        private ICommand m_AddTileCommand;

        #endregion

        #region Ctors

        public Main()
        {
            Tiles = new ObservableCollection<object>();
            m_HeightIncrementCommand = ReactiveCommand.Create(IncrementHeight, null, RxApp.MainThreadScheduler);
            m_AddTileCommand = ReactiveCommand.Create(AddTile, null, RxApp.MainThreadScheduler);
        }

        #endregion

        #region Properties

        public double MyHeight
        {
            get
            {
                return m_MyHeight;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                m_MyHeight = value;
                this.RaisePropertyChanged(nameof(MyHeight));
            }
        }

        public double HeightIncrement
        {
            get
            {
                return m_HeightIncrement;
            }
            set
            {
                m_HeightIncrement = value;
                this.RaisePropertyChanged(nameof(HeightIncrement));
            }
        }

        public ObservableCollection<object> Tiles { get; }

        public int MyTabIndex
        {
            get
            {
                return m_MyTabIndex;
            }
            set
            {
                m_MyTabIndex = value;
                this.RaisePropertyChanged(nameof(MyTabIndex));
            }
        }

        public ICommand HeightIncrementCommand
        {
            get
            {
                return m_HeightIncrementCommand;
            }
        }

        public ICommand AddTileCommand
        {
            get
            {
                return m_AddTileCommand;
            }
        }

        #endregion

        #region Public Methods

        public void IncrementHeight()
        {
            MyHeight += HeightIncrement;
        }

        public void AddTile()
        {
            Tiles.Add(new object());
        }

        #endregion
    }
}
