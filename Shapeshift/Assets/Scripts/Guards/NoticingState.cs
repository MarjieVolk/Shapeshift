﻿using UnityEngine;
using System.Collections;

public class NoticingState : State {
    /// <summary>
    /// The amount of time the player needs to be noticed for before alarming.
    /// </summary>
    public float TotalNoticedTime = 0.5f;

    /// <summary>
    /// The amount of time since the last notice event before the guard starts losing suspicion.
    /// </summary>
    public float NoticeResetDelay = 1.0f;

    /// <summary>
    /// The amount of time it would take to reset a fully suspicious guard.
    /// </summary>
    public float NoticeResetTime = 1.0f;

    private float noticeTime = 0;
    private float lastNoticeTime;

    public bool HandlePlayerDetected()
    {
        if (GetComponent<ChaseState>().enabled)
        {
            noticeTime = 0;
            return true;
        }

        Debug.Log("Player detected!");

        noticeTime += Time.deltaTime;
        lastNoticeTime = Time.time;

        // switch to this state if we just noticed the player
        if (!this.enabled)
        {
            GetComponent<StateMachine>().CurrentState = this;
        }

        return false;
    }

    void OnEnable()
    {
        noticeTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        // Start chasing after some time elapses
        if (Time.time >= lastNoticeTime + NoticeResetDelay)
        {
            noticeTime -= Time.deltaTime / NoticeResetTime;
            if (noticeTime <= 0)
            {
                noticeTime = 0;
                GetComponent<StateMachine>().CurrentState = GetComponent<LookState>();
            }
        }

        if (noticeTime >= TotalNoticedTime)
        {
            noticeTime = TotalNoticedTime;
            GetComponent<ChaseState>().HandlePlayerSpotted(FindObjectOfType<PlayerTransformer>().transform.position);
            GetComponent<StateMachine>().CurrentState = GetComponent<ChaseState>();
        }
	}
}
