using Other;
using UnityEngine;

namespace ScriptsToTest
{
    public enum DerivativeType
    {
        ErrorRate,
        Velocity
    }
    
    public class PIDController
    {
        public float ProportionalGain { get; set; }
        public float IntegralGain { get; set; }
        public float DerivativeGain { get; set; }
        public DerivativeType DerivativeType { get; set; }

        public float errorLast;
        public float valueLast;
        public float integrationStored;
        public float integrationMax;
        
        public float LastP = 0;
        public float LastI = 0;
        public float LastD = 0;
        
        private bool DerivativeStepZeroPassed = false;
        
        public PIDController() { }

        public void Initialize(float p, float i, float d, DerivativeType derivativeType)
        {
            ProportionalGain = p;
            IntegralGain = i;
            DerivativeGain = d;
            DerivativeType = derivativeType;
            DerivativeStepZeroPassed = false;
            integrationMax = 0.2f / IntegralGain;
        }

        public void Reset()
        {
            DerivativeStepZeroPassed = false;
            errorLast = 0;
            valueLast = 0;
            integrationStored = 0;
        }
        
        public float Update(float dt, float currentValue, float targetValue)
        {
            float error = targetValue - currentValue;
            
            // P
            float P = ProportionalGain  * error;
            LastP = P;

            // I
            integrationStored = Mathf.Clamp( integrationStored + (error * dt), -integrationMax, integrationMax);
            
            float I = integrationStored * IntegralGain;
            LastI = I;
            
            // D
            float rateOfErrorChange = (error - errorLast) / dt;
            errorLast = error;
            
            float rateOfValueChange = (currentValue - valueLast) / dt;
            valueLast = currentValue;
    
            float DMeasure = 0;

            if (DerivativeStepZeroPassed)
            {
                if (DerivativeType == DerivativeType.Velocity) {
                    DMeasure = -rateOfValueChange;
                }
                else {
                    DMeasure = rateOfErrorChange;
                }
            }
            else
            {
                DerivativeStepZeroPassed = true;
            }
            
            float D = DerivativeGain * DMeasure;
            LastD = D;

            float result = P + I + D;
            Debug.LogWarning($"{result}");
            return result;
        }

        public struct PID
        {
            public float P;
            public float I;
            public float D;

            public PID(float p, float i, float d)
            {
                P = p;
                I = i;
                D = d;
            }
        }
    }
}