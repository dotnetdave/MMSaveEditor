﻿using FullSerializer;
using System;
using System.Collections.Generic;

[fsObject(MemberSerialization = fsMemberSerialization.OptOut)]
public class ChallengeManager
{
  private List<Challenge> mChallenges = new List<Challenge>();
  private Challenge mCurrentChallenge;

  public enum ChallengeManagerGameEvents
  {
    NewCareer,
    PlayerTryingToLeaveTeam,
    PlayerLeftTeam,
    WonTopTier,
  }
}