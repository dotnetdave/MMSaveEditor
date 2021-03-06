﻿using System;
using FullSerializer;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using MMSaveEditor.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using MMSaveEditor.View;
using MMSaveEditor.Utils;

[fsObject(MemberSerialization = fsMemberSerialization.OptOut)]
public class Driver : Person
{
    public DriverCareerForm careerForm = new DriverCareerForm();
    public DriverMentalState mentalState = new DriverMentalState();
    public CarOpinion carOpinion = new CarOpinion();
    public DriverStamina driverStamina;
    public DriverForm driverForm;
    public int driverNumber;
    public int desiredChampionships = RandomUtility.GetRandom(1, 4);
    public long desiredBudget = (long)RandomUtility.GetRandom(100, 10000) * 1000L;
    private int mDesiredWins = RandomUtility.GetRandom(1, 100);
    private long mDesiredEarnings = (long)RandomUtility.GetRandom(100, 1000) * 1000L;
    public int startOfSeasonExpectedChampionshipPosition;

    public int expectedChampionshipPosition;
    public int expectedRacePosition;
    public PersonalityTraitController_v2 personalityTraitController;
    [NonSerialized]
    private ChampionshipEntry_v1 mChampionshipEntry;
    private DriverStats accumulatedStats = new DriverStats();
    public DriverStats lastAccumulatedStats = new DriverStats();
    public DriverStats statsBeforeEvent;
    public float moraleBeforeEvent;
    public float championshipExpectation;
    public float raceExpectation;
    private bool mJoinsAnySeries = true;
    private Championship.Series mPreferedSeries;

    private List<Championship.Series> mDriverPreferedSeries = new List<Championship.Series>()
  {
    Championship.Series.SingleSeaterSeries
  };
    private DriverStats mStats = new DriverStats();
    private DriverStats mModifiedStats = new DriverStats();

    private float mImprovementRate = RandomUtility.GetRandom(0.1f, 1f);
    private float mPotential;
    private float mModifiedPotential;
    private bool mHasBeenScouted;
    private bool mIsReplacementDriver;
    private bool mHasCachedReplacementDriverInfo;
    private int mLastRaceExpectedRacePosition;
    private Person mCelebrity;
    private DriverRivalries mDriverRivalries = new DriverRivalries();
    private int mDaysToScoutShort = RandomUtility.GetRandomInc(10, 20);

    private int mDaysToScoutLong = RandomUtility.GetRandomInc(20, 45);
    private DateTime lowMoraleStartTime = new DateTime();
    private DateTime mLastMoraleBonusDate = new DateTime();
    private int mCarID = -1;
    private readonly int moraleBonusCooldownDays = 30;



    private readonly float moralePromotionBonus = 0.4f;
    private readonly float moraleDemotionBonus = -0.4f;
    private readonly float moraleBetterContractBonus = 0.4f;
    private readonly float moraleWorseContractBonus = -0.4f;
    private readonly float moraleSignedContractBonus = 0.4f;
    private readonly float moraleFiredBonus = -0.4f;
    private readonly float moraleBetterContractPerRace = 0.025f;
    private readonly float moraleWorseContractPerRace = -0.025f;
    private readonly int lowMoraleOpenToOffersDays = 50;
    private readonly float lowMoraleOpenToOffersAmount = 0.1f;
    private readonly float lowMoraleStopListeningToOffersAmount = 0.3f;
    private readonly float moraleAchieveExpectedPositionBonus = 0.025f;
    private readonly float moraleFailedExpectedPositionBonus = -0.025f;
    private readonly float moraleSessionPodiumBonus = 0.03f;
    private readonly float moraleChampionshipPositionNormalModifier = 1f;
    private readonly float moraleKeptChampionshipExpectedPositionModifier = 0.1f;
    private readonly float moraleMinSessionChange = -0.2f;
    private readonly float moraleMaxSessionChange = 0.2f;
    private readonly float moralePracticeWeight = 0.05f;
    private readonly float moraleQualifyingWeight = 0.3f;
    private readonly float moraleRaceWeight = 0.65f;
    private readonly float moraleRacePerformanceWeight = 1f;
    private readonly float moraleGoalsWeight = 0.05f;
    private readonly float negativeImprovementHQScalar = 0.9f;
    private readonly float negativeImprovementHQOverallScalar = 0.03f;
    private readonly float negativeMaxImprovementHQ = 0.75f;






    public bool hasBeenScouted
    {
        get => this.mHasBeenScouted;
        set => mHasBeenScouted = value;
    }

    public int daysToScoutShort
    {
        get => this.mDaysToScoutShort;
        set => mDaysToScoutShort = value;
    }

    public int daysToScoutLong
    {
        get => this.mDaysToScoutLong;
        set => mDaysToScoutLong = value;
    }

    public Championship.Series preferedSeries
    {
        get => this.mPreferedSeries;
        set => mPreferedSeries = value;
    }
    public IEnumerable<Championship.Series> SeriesTypes => Enum.GetValues(typeof(Championship.Series)).Cast<Championship.Series>();

    public bool joinsAnySeries
    {
        get => this.mJoinsAnySeries;
        set => mJoinsAnySeries = value;
    }

    public int DesiredChampionships
    {
        get => desiredChampionships;

        set => desiredChampionships = value;
    }

    public long DesiredBudget
    {
        get => desiredBudget;

        set => desiredBudget = value;
    }

    public int MDesiredWins
    {
        get => mDesiredWins;

        set => mDesiredWins = value;
    }

    public long MDesiredEarnings
    {
        get => mDesiredEarnings;

        set => mDesiredEarnings = value;
    }

    public int DriverNumber
    {
        get => driverNumber;

        set => driverNumber = value;
    }

    public int StartOfSeasonExpectedChampionshipPosition
    {
        get => startOfSeasonExpectedChampionshipPosition;

        set => startOfSeasonExpectedChampionshipPosition = value;
    }

    public int ExpectedChampionshipPosition
    {
        get => expectedChampionshipPosition;

        set => expectedChampionshipPosition = value;
    }

    public int ExpectedRacePosition
    {
        get => expectedRacePosition;

        set => expectedRacePosition = value;
    }

    public float MPotential
    {
        get => mPotential;

        set => mPotential = value;
    }

    public float MModifiedPotential
    {
        get => mModifiedPotential;

        set => mModifiedPotential = value;
    }

    public DriverStats MStats
    {
        get => mStats;

        set => mStats = value;
    }

    public RelayCommand<Driver> ViewDriver { get; private set; }
    public RelayCommand<PersonalityTraitData> AddTrait { get; private set; }

    public PersonalityTraitController_v2 PersonalityTraitController
    {
        get
        {
            return personalityTraitController;
        }

        set
        {
            personalityTraitController = value;
        }
    }

    public float ImprovementRate
    {
        get
        {
            return mImprovementRate;
        }

        set
        {
            mImprovementRate = value;
        }
    }

    public int carID
    {
        get
        {
            if (this.mCarID >= 0)
                return this.mCarID;
            return this.GetCarID();
        }
    }

    public void ResetChampionshipEntry()
    {
        this.mChampionshipEntry = (ChampionshipEntry_v1)null;
    }

    private int GetCarID()
    {
        if (this.IsReserveDriver())
            return -1;
        return this.contract.GetTeam().GetDriver(0) == this ? 0 : 1;
    }

    public void SetCarID(int inCarID)
    {
        this.mCarID = inCarID.Clamp(-1, 1);
    }

    public bool IsReserveDriver()
    {
        return this.contract.currentStatus == ContractPerson.Status.Reserve;
    }

    public Driver()
    {
        ViewDriver = new RelayCommand<Driver>(_viewDriver);
        AddTrait = new RelayCommand<PersonalityTraitData>(addTrait);
    }
    private void _viewDriver(Driver d)
    {
        var driverVM = SimpleIoc.Default.GetInstance<DriverViewModel>();
        driverVM.SetModel(this);
        MainWindow.Instance.SwitchToTab(MainWindow.TabPage.Driver);
    }

    public void addTrait(PersonalityTraitData data)
    {
        if (data != null)
            personalityTraitController.AddPersonalityTrait(data, true);
    }

    public void removeTrait(PersonalityTrait data)
    {
        if (data != null)
            personalityTraitController.RemovePersonalityTrait(data);
    }

    public void UpdateModifiedPotentialValue(float inPotentialModifier)
    {
        this.mModifiedPotential += inPotentialModifier;
        this.mModifiedPotential = this.mModifiedPotential.Clamp(0.0f, (float)this.mStats.GetMaxPotential());
        this.mStats.SetMaxFromPotential((int)this.mModifiedPotential);
        this.mModifiedStats.totalStatsMax = this.mStats.totalStatsMax;
    }

    public override bool IsReplacementPerson()
    {
        if (!this.mHasCachedReplacementDriverInfo)
        {
            this.mHasCachedReplacementDriverInfo = true;
            this.mIsReplacementDriver = Game.instance.driverManager.IsReplacementPerson(this);
        }
        return this.mIsReplacementDriver;
    }

    internal bool IsMainDriver()
    {
        return this.Contract.currentStatus != ContractPerson.Status.Reserve;
    }

    public ChampionshipEntry_v1 GetChampionshipEntry()
    {
        if (!this.IsFreeAgent())
        {
            Championship championship = this.Contract.GetTeam().championship;
            if (this.mChampionshipEntry == null || this.mChampionshipEntry.championship != championship)
                this.mChampionshipEntry = championship.standings.GetEntry((Entity)this);
        }
        return this.mChampionshipEntry;
    }

    public void SetBeenScouted()
    {
        this.mHasBeenScouted = true;
    }

    public DriverStats GetDriverStats()
    {
        return (DriverStats)this.GetStats();
    }

    public override PersonStats GetStats()
    {
        DriverStats inAdd = (DriverStats)null;
        if (this.personalityTraitController != null)
            inAdd = this.personalityTraitController.GetDriverStatsModifier();
        else
            Console.WriteLine("{0} does not have  personality trait controller", this.name);
        this.mModifiedStats.Clear();
        if (inAdd != null)
            this.mModifiedStats.Add(inAdd);
        this.mModifiedStats.Add(this.mStats);
        this.mModifiedStats.marketability += inAdd.marketability;
        this.mModifiedStats.marketability += this.mStats.marketability;
        if (this.IsPlayersDriver())
            this.mModifiedStats.feedback += Game.instance.player.driverFeedBackStatModifier;
        this.mModifiedStats.ClampStats();
        return (PersonStats)this.mModifiedStats;
    }

    public bool IsPlayersDriver()
    {
        if (this.contract != null)
            return this.contract.GetTeam().IsPlayersTeam();
        return false;
    }
}
