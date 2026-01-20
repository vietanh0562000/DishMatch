using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserProfile
{
    public string name;
    public int avatar_id;
    public string avatar_url;
    public int frame_id;
    public int badge;

    public void Update(UserProfile userInfo)
    {
        this.name = userInfo.name;
        this.avatar_id = userInfo.avatar_id;
        this.avatar_url = userInfo.avatar_url;
        this.frame_id = userInfo.frame_id;
        this.badge = userInfo.badge;
    }
}

[Serializable]
public class UserSimpleInfo : UserProfile
{
    public int code;
    public int level;
    public int golden;

    public UserSimpleInfo()
    {
    }

    public bool IsGoldenFrame()
    {
        return this.golden > 0;
    }
}

[Serializable]
public class UserSimpleInfoWithTeam : UserSimpleInfo
{
    //public TeamInfoSimple teamInfo;
}

[Serializable]
public class UserInfoSearch : UserSimpleInfo
{
    //public TeamInfoSimple teamInfo;
    public int friendStatus;
}


[Serializable]
public class UserInfoDetail : UserSimpleInfo
{
    //public TeamInfoSimple teamInfo;
    public int help_made;
    public int help_received;
    public long createdDate;
    public int firstTryWins;
    public int friendStatus;

    public int collectionCompleted;
    public int setCompleted;

    public FriendStatus GetFriendStatus()
    {
        return (FriendStatus)friendStatus;
    }

    public string GetCreatedDate()
    {
        var dateTime = DateTimeUtils.GetDateTimeFromMiliSecond(createdDate);
        return string.Format("{0}/{1}", dateTime.Month, dateTime.Year);
    }

    public int GetHelpMade()
    {
        return help_made;
    }

    public int GetHelpReceived()
    {
        return help_received;
    }

    public int GetFirstTryWins()
    {
        return 0;
    }

    public int GetAreasCompleted()
    {
        return 0;
    }

    public int GetCollectionsCompleted()
    {
        return collectionCompleted;
    }

    public int GetSetsCompleted()
    {
        return setCompleted;
    }
}

public enum FriendStatus
{
    NOT_FRIEND,

    REQUESTED,
    PENDING,

    FRIEND
}

[Serializable]
public class UserInfoLeaderboard : UserInfoScore
{
   // public TeamInfoSimple teamInfo;
}

[Serializable]
public class UserInfoTeamBattle : UserInfoScore
{
}

[Serializable]
public class UserInfoTeamTreasure : UserInfoScore
{
    public long lastScoreTimestamp;
}

[Serializable]
public class UserInfoScore : UserSimpleInfo
{
    public int score;
    public int rank;
}

[Serializable]
public class UserInfoStarCollab : UserSimpleInfo
{
    public int myScore;
    public int partnerScore;

    public int preScore;
    public int score;

    public int milkFromPartner;

    public int state;
    public List<int> rewards;

    public UserInfoStarCollab()
    {
    }
}