using DosProtection.AspNetApi.Dynamic;

namespace TestEnvironment.Server.Services;

public class StressAnalyzer : IServerStressProvider
{
    private float _currentStress = 0f;
    public float GetStress() => Math.Clamp(_currentStress, 0f, 1f);
    public float SetStress(float value) => _currentStress = Math.Clamp(value, 0f, 1f);
}
