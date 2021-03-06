﻿using System;
using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;

namespace MMSaveEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        private GameTimer _timeData;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public GameViewModel()
        {

        }

        public void SetModels(GameTimer timeData)
        {
            _timeData = timeData;
            RaisePropertyChanged(String.Empty);
        }

        public DateTime DateNow
        {
            get
            {
                if (_timeData != null)
                    return _timeData.now;
                return DateTime.Now;
            }
            set
            {
                _timeData.now = value;
                RaisePropertyChanged(String.Empty);
            }
        }


    }
}
