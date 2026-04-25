#if UNITY_INCLUDE_TESTS
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ParticleThumbnailAndPreview.Editor.Tests
{
    public sealed class ParticleRenderCompatibilityUtilityTests
    {
        [Test]
        public void PipelineClassification_RecognizesKnownPipelines()
        {
            Assert.AreEqual(
                ParticleRenderPipelineKind.BuiltIn,
                ParticleRenderCompatibilityUtility.ClassifyPipelineKindForTests(string.Empty, null));

            Assert.AreEqual(
                ParticleRenderPipelineKind.Urp3D,
                ParticleRenderCompatibilityUtility.ClassifyPipelineKindForTests(
                    "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset",
                    "UnityEngine.Rendering.Universal.ForwardRendererData"));

            Assert.AreEqual(
                ParticleRenderPipelineKind.Urp2D,
                ParticleRenderCompatibilityUtility.ClassifyPipelineKindForTests(
                    "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset",
                    "UnityEngine.Rendering.Universal.Renderer2DData"));

            Assert.AreEqual(
                ParticleRenderPipelineKind.Hdrp,
                ParticleRenderCompatibilityUtility.ClassifyPipelineKindForTests(
                    "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset",
                    null));

            Assert.AreEqual(
                ParticleRenderPipelineKind.UnknownSrp,
                ParticleRenderCompatibilityUtility.ClassifyPipelineKindForTests(
                    "CustomCompany.CustomPipelineAsset",
                    null));
        }

        [Test]
        public void RenderPreference_PrefersSrpForNonBuiltInPipelines()
        {
            Assert.IsFalse(ParticleRenderCompatibilityUtility.ShouldPreferSrpRenderForTests(ParticleRenderPipelineKind.BuiltIn));
            Assert.IsTrue(ParticleRenderCompatibilityUtility.ShouldPreferSrpRenderForTests(ParticleRenderPipelineKind.Urp3D));
            Assert.IsTrue(ParticleRenderCompatibilityUtility.ShouldPreferSrpRenderForTests(ParticleRenderPipelineKind.Urp2D));
            Assert.IsTrue(ParticleRenderCompatibilityUtility.ShouldPreferSrpRenderForTests(ParticleRenderPipelineKind.Hdrp));
            Assert.IsTrue(ParticleRenderCompatibilityUtility.ShouldPreferSrpRenderForTests(ParticleRenderPipelineKind.UnknownSrp));
        }

        [Test]
        public void ShaderTimeScope_RestoresPreviousValues_OnException()
        {
            Vector4 originalTime = new Vector4(0.1f, 1.2f, 2.3f, 3.4f);
            Vector4 originalSinTime = new Vector4(0.2f, 1.3f, 2.4f, 3.5f);
            Vector4 originalCosTime = new Vector4(0.3f, 1.4f, 2.5f, 3.6f);
            Vector4 originalDeltaTime = new Vector4(0.4f, 1.5f, 2.6f, 3.7f);

            Shader.SetGlobalVector("_Time", originalTime);
            Shader.SetGlobalVector("_SinTime", originalSinTime);
            Shader.SetGlobalVector("_CosTime", originalCosTime);
            Shader.SetGlobalVector("unity_DeltaTime", originalDeltaTime);

            try
            {
                using (ParticleRenderCompatibilityUtility.PushShaderTime(1.25f))
                {
                    throw new InvalidOperationException("Intentional test exception.");
                }
            }
            catch (InvalidOperationException)
            {
                // Expected.
            }

            Assert.That(Shader.GetGlobalVector("_Time"), Is.EqualTo(originalTime));
            Assert.That(Shader.GetGlobalVector("_SinTime"), Is.EqualTo(originalSinTime));
            Assert.That(Shader.GetGlobalVector("_CosTime"), Is.EqualTo(originalCosTime));
            Assert.That(Shader.GetGlobalVector("unity_DeltaTime"), Is.EqualTo(originalDeltaTime));
        }

        [Test]
        public void RenderOverloadHook_CanBeQueried()
        {
            bool hasSrpOverload = ParticleRenderCompatibilityUtility.HasSrpRenderOverloadForTests;
            Assert.That(hasSrpOverload == true || hasSrpOverload == false);
        }

        [Test]
        public void RendererScope_RestoresRendererEnabledState()
        {
            GameObject goA = new GameObject("RendererScopeTestA");
            GameObject goB = new GameObject("RendererScopeTestB");
            try
            {
                Renderer rendererA = goA.AddComponent<MeshRenderer>();
                Renderer rendererB = goB.AddComponent<MeshRenderer>();
                rendererA.enabled = false;
                rendererB.enabled = true;

                var renderers = new List<Renderer> { rendererA, rendererB, null };
                using (ParticleRenderCompatibilityUtility.EnableRenderersScoped(renderers))
                {
                    Assert.IsTrue(rendererA.enabled);
                    Assert.IsTrue(rendererB.enabled);
                }

                Assert.IsFalse(rendererA.enabled);
                Assert.IsTrue(rendererB.enabled);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(goA);
                UnityEngine.Object.DestroyImmediate(goB);
            }
        }
    }
}
#endif
