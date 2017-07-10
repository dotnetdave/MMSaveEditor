﻿using System;

public abstract class Challenge
{
  public string descriptionID = string.Empty;
  public string objectivesID = string.Empty;
  public string restrictionsID = string.Empty;
  public string rewardsID = string.Empty;
  public Challenge.ChallengeName challengeName;
  public Challenge.Difficulty difficulty;
  public Challenge.ChallengeStatus status;
  public ChallengeRestrictions restrictions;
    
  public enum Difficulty
  {
    [LocalisationID("PSG_10010654")] Easy,
    [LocalisationID("PSG_10010655")] Medium,
    [LocalisationID("PSG_10010656")] Hard,
  }

  public enum ChallengeName
  {
    [LocalisationID("PSG_10007208")] Underdog,
    [LocalisationID("PSG_10007214")] TopManager,
  }

  public enum ChallengeStatus
  {
    [LocalisationID("PSG_10009316")] InProgress,
    [LocalisationID("PSG_10008896")] Completed,
    [LocalisationID("PSG_10007186")] Failed,
  }
}