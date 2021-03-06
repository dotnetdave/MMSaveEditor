﻿
using FullSerializer;
using System;
using System.Collections.Generic;

[fsObject(MemberSerialization = fsMemberSerialization.OptOut)]
public class RaceEventDetails
{
    public DateTime eventDate;
    public List<SessionDetails> sessions = new List<SessionDetails>();
    public RaceEventResults results = new RaceEventResults();
    public Circuit circuit;
    private int mSessionID;
    private int mNextSessionID;
    private bool mIsEventEnding;
    private bool mHasEventEnded;
    private int mSessionPointer;
    private int mNextSessionPointer;
    private List<SessionDetails> mPracticeSessions;
    private List<SessionDetails> mQualifyingSessions;
    private List<SessionDetails> mRaceSessions;

    public List<SessionDetails> raceSessions
    {
        get
        {
            if (this.mRaceSessions == null)
                this.mRaceSessions = this.sessions.FindAll((Predicate<SessionDetails>)(session => session.sessionType == SessionDetails.SessionType.Race));
            return this.mRaceSessions;
        }
    }

    public SessionDetails currentSession
    {
        get
        {
            if (this.mSessionPointer >= this.sessions.Count)
                this.SetPointerToCorrectSession();
            return this.sessions[this.mSessionPointer];
        }
    }

    private void SetPointerToCorrectSession()
    {
        for (int index = 0; index < this.sessions.Count; ++index)
        {
            this.mSessionPointer = index;
            if (!this.sessions[index].hasEnded)
                break;
        }
    }
}
