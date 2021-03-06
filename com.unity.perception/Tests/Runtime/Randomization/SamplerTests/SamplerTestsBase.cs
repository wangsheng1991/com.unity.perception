﻿using System;
using NUnit.Framework;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Perception.Randomization.Samplers;
using UnityEngine.Experimental.Perception.Randomization.Scenarios;
using Object = UnityEngine.Object;

namespace RandomizationTests.SamplerTests
{
    public abstract class SamplerTestsBase<T> where T : ISampler
    {
        protected const int k_TestSampleCount = 30;
        protected T m_BaseSampler;
        protected T m_Sampler;
        GameObject m_ScenarioObj;

        static ScenarioBase activeScenario => ScenarioBase.activeScenario;

        [SetUp]
        public void Setup()
        {
            m_Sampler = m_BaseSampler;
            m_ScenarioObj = new GameObject("Scenario");
            m_ScenarioObj.AddComponent<FixedLengthScenario>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_ScenarioObj);
        }

        [Test]
        public void ConsecutiveSamplesChangesState()
        {
            var state0 = activeScenario.randomState;
            m_Sampler.Sample();
            var state1 = activeScenario.randomState;
            m_Sampler.Sample();
            var state2 = activeScenario.randomState;;

            Assert.AreNotEqual(state0, state1);
            Assert.AreNotEqual(state1, state2);
        }
    }
}
