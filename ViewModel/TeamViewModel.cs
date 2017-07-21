﻿using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace MMSaveEditor.ViewModel
{
    public class TeamViewModel : ViewModelBase
    {
        private Team teamData;

        public ObservableCollection<Team> Teams => Game.Instance == null ? null : new ObservableCollection<Team>(Game.Instance?.teamManager?.GetEntityList());
        public ObservableCollection<CarPart> BrakesGT => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.brakesGTInventory);
        public ObservableCollection<CarPart> Brakes => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.brakesInventory);
        public ObservableCollection<CarPart> EngineGT => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.engineGTInventory);
        public ObservableCollection<CarPart> Engine => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.engineInventory);
        public ObservableCollection<CarPart> FrontWing => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.frontWingInventory);
        public ObservableCollection<CarPart> GearboxGT => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.gearboxGTInventory);
        public ObservableCollection<CarPart> Gearbox => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.gearboxInventory);
        public ObservableCollection<CarPart> RearWingGT => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.rearWingGTInventory);
        public ObservableCollection<CarPart> RearWing => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.rearWingInventory);
        public ObservableCollection<CarPart> SuspensionGT => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.suspensionGTInventory);
        public ObservableCollection<CarPart> Suspension => TeamData == null ? null : new ObservableCollection<CarPart>(TeamData?.carManager?.partInventory?.suspensionInventory);

        public int Reputation
        {
            get { return TeamData?.reputation ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.reputation = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }

        public float Marketability
        {
            get { return TeamData?.marketability ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.marketability = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }
        public int Pressure
        {
            get { return TeamData?.pressure ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.pressure = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }
        public float FanBase
        {
            get { return TeamData?.fanBase ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.fanBase = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }
        public float Aggression
        {
            get { return TeamData?.aggression ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.aggression = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }
        public float InitialTotalFanBase
        {
            get { return TeamData?.initialTotalFanBase ?? 0; }
            set
            {
                if (TeamData != null)
                {
                    TeamData.initialTotalFanBase = value;
                    RaisePropertyChanged(String.Empty);
                }
            }
        }

        public Team TeamData
        {
            get
            {
                return teamData;
            }

            set
            {
                teamData = value;
            }
        }

        public void SetModel(Team targetTeam)
        {
            TeamData = targetTeam;
            RaisePropertyChanged(String.Empty);
        }
    }
}
