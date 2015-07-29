//------------------------------------------------------------------------------
// <copyright file="TimedLerp.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using UnityEngine;
using System;

/// <summary>
/// TimedLerp - Maintains a time-based lerp between 0 and a upper limit between 0 and 1.
/// The lerp speed parameter is in units of inverse time - therefore, a speed of 2.0
/// means that the lerp completes a full transition (0 to 1) in 0.5 seconds.
/// </summary>
public class TimedLerp
{
    // Enabled value.
    protected float Enabled;

    // The Value.
    protected float Value;

    // Ease in speed.
    protected float EaseInSpeed;

    // Ease out speed.
    protected float EaseOutSpeed;
	

    // Initializes a new instance of the class.
    public TimedLerp()
    {
        this.Enabled = 0.0f;
        this.Value = 0.0f;
        this.EaseInSpeed = 1.0f;
        this.EaseOutSpeed = 1.0f;
    }

    // Gets LinearValue. Returns a raw, linearly interpolated value between 0 and the Math.Maximum value.
    public float LinearValue
    {
        get
        {
            return this.Value;
        }
    }

    // Gets SmoothValue. Returns the value between 0 and the Math.Maximum value, but applies a cosine-shaped smoothing function.
    public float SmoothValue
    {
        get
        {
            return 0.5f - (0.5f * (float)Mathf.Cos(this.Value * Mathf.PI));
        }
    }

    // Set default in/out speeds.
    public void SetSpeed()
    {
        this.SetSpeed(0.5f, 0.0f);
    }

    /// Set in/out speeds.
    public void SetSpeed(float easeInSpeed, float easeOutSpeed) 
    {
        this.EaseInSpeed = easeInSpeed;
		
        if (easeOutSpeed <= 0.0f)
        {
            this.EaseOutSpeed = easeInSpeed;
        }
        else
        {
            this.EaseOutSpeed = easeOutSpeed;
        }
    }

    // Set whether the Lerp is enabled.
    public void SetEnabled(bool isEnabled)
    {
        this.Enabled = isEnabled ? 1.0f : 0.0f;
    }

    // Set the Lerp enable value.
    public void SetEnabled(float enabled)
    {
        this.Enabled = Math.Max(0.0f, Math.Min(1.0f, enabled));
    }

    // ReSet the Lerp.
    public void Reset()
    {
        this.Enabled = 0.0f;
        this.Value = 0.0f;
    }

    // IsEnabled reflects whether the target value is 0 or not.
    public bool IsEnabled()
    {
        return this.Enabled > 0.0f;
    }

    /// IsLerpEnabled reflects whether the current value is 0 or not.
    public bool IsLerpEnabled()
    {
        return this.IsEnabled() || (this.Value > 0.0f);
    }

    /// Tick needs to be called once per frame.
    public void Tick(float deltaTime)
    {
        float speed = this.EaseInSpeed;
		
        if (this.Value > this.Enabled)
        {
            speed = this.EaseOutSpeed;
        }

        float delta = speed * deltaTime;
		
        if (this.Enabled > 0.0f)
        {
            this.Value = Math.Min(this.Enabled, this.Value + delta);
        }
        else
        {
            this.Value = Math.Max(0.0f, this.Value - delta);
        }
    }
}

