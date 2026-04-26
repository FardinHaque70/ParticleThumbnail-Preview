using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ParticleThumbnailAndPreview.Editor
{
	[FilePath(SettingsPath, FilePathAttribute.Location.ProjectFolder)]
	internal sealed class ParticleThumbnailSettingsStorage : ScriptableSingleton<ParticleThumbnailSettingsStorage>
	{
		private const string SettingsPath = "ProjectSettings/ParticleThumbnailAndPreview/ParticleThumbnailSettings.asset";

		[SerializeField] internal bool enabled = true;
		[SerializeField] internal bool drawInProjectGrid = true;
		[SerializeField] internal bool drawInProjectList = true;

		[SerializeField] internal int gridRenderSize = ParticleThumbnailSettings.D_GridRenderSize;
		[SerializeField] internal int listRenderSize = ParticleThumbnailSettings.D_ListRenderSize;
		[SerializeField] internal Color backgroundColor = ParticleThumbnailSettings.D_BackgroundColor;

		[SerializeField] internal float cameraFov = ParticleThumbnailSettings.D_CameraFov;
		[SerializeField] internal float cameraYaw = ParticleThumbnailSettings.D_CameraYaw;
		[SerializeField] internal float cameraPitch = ParticleThumbnailSettings.D_CameraPitch;
		[SerializeField] internal float boundsPadding = ParticleThumbnailSettings.D_BoundsPadding;

		[SerializeField] internal float scanMaxSeconds = ParticleThumbnailSettings.D_ScanMaxSeconds;
		[SerializeField] internal float motionPadding = ParticleThumbnailSettings.D_MotionPadding;
		[SerializeField] internal float motionRadius = ParticleThumbnailSettings.D_MotionRadius;
		[SerializeField] internal float motionSpeed = ParticleThumbnailSettings.D_MotionSpeed;
		[SerializeField] internal float thumbnailFillTarget = ParticleThumbnailSettings.D_ThumbnailFillTarget;
		[SerializeField] internal bool enableTightFraming = ParticleThumbnailSettings.D_EnableTightFraming;
		[SerializeField] internal float particleFramingPercentile = ParticleThumbnailSettings.D_ParticleFramingPercentile;

		[SerializeField] internal int maxRendersPerUpdate = ParticleThumbnailSettings.D_MaxRendersPerUpdate;
		[SerializeField] internal float renderBudgetMs = ParticleThumbnailSettings.D_RenderBudgetMs;
		[SerializeField] internal int memoryCacheMaxEntries = ParticleThumbnailSettings.D_MemoryCacheMaxEntries;
		[SerializeField] internal bool enablePersistentCache = true;

		internal void SaveStorage()
		{
			string directory = Path.GetDirectoryName(SettingsPath);
			if (!string.IsNullOrEmpty(directory))
				Directory.CreateDirectory(directory);

			Save(true);
		}

		internal void ResetToDefaults()
		{
			enabled = true;
			drawInProjectGrid = true;
			drawInProjectList = true;

			gridRenderSize = ParticleThumbnailSettings.D_GridRenderSize;
			listRenderSize = ParticleThumbnailSettings.D_ListRenderSize;
			backgroundColor = ParticleThumbnailSettings.D_BackgroundColor;

			cameraFov = ParticleThumbnailSettings.D_CameraFov;
			cameraYaw = ParticleThumbnailSettings.D_CameraYaw;
			cameraPitch = ParticleThumbnailSettings.D_CameraPitch;
			boundsPadding = ParticleThumbnailSettings.D_BoundsPadding;

			scanMaxSeconds = ParticleThumbnailSettings.D_ScanMaxSeconds;
			motionPadding = ParticleThumbnailSettings.D_MotionPadding;
			motionRadius = ParticleThumbnailSettings.D_MotionRadius;
			motionSpeed = ParticleThumbnailSettings.D_MotionSpeed;
			thumbnailFillTarget = ParticleThumbnailSettings.D_ThumbnailFillTarget;
			enableTightFraming = ParticleThumbnailSettings.D_EnableTightFraming;
			particleFramingPercentile = ParticleThumbnailSettings.D_ParticleFramingPercentile;

			maxRendersPerUpdate = ParticleThumbnailSettings.D_MaxRendersPerUpdate;
			renderBudgetMs = ParticleThumbnailSettings.D_RenderBudgetMs;
			memoryCacheMaxEntries = ParticleThumbnailSettings.D_MemoryCacheMaxEntries;
			enablePersistentCache = true;
		}
	}

	internal static class ParticleThumbnailSettings
	{
		public const int D_GridRenderSize = 128;
		public const int D_ListRenderSize = 32;
		public static readonly Color D_BackgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);

		public const float D_CameraFov = 30f;
		public const float D_CameraYaw = 35f;
		public const float D_CameraPitch = 25f;
		public const float D_BoundsPadding = 0.15f;

		public const float D_ScanMaxSeconds = 3f;
		public const float D_MotionPadding = 0.2f;
		public const float D_MotionRadius = 3f;
		public const float D_MotionSpeed = 60f;
		public const float D_ThumbnailFillTarget = 0.72f;
		public const bool D_EnableTightFraming = true;
		public const float D_ParticleFramingPercentile = 0.92f;

		public const int D_MaxRendersPerUpdate = 1;
		public const float D_RenderBudgetMs = 12f;
		public const int D_MemoryCacheMaxEntries = 200;

		private const string SettingsTokenVersion = "particle-thumb-v2";

		public static event Action SettingsChanged;

		private static ParticleThumbnailSettingsStorage Storage => ParticleThumbnailSettingsStorage.instance;

		public static bool Enabled => Storage.enabled;
		public static bool DrawInProjectGrid => Storage.drawInProjectGrid;
		public static bool DrawInProjectList => Storage.drawInProjectList;

		public static int GridRenderSize => Mathf.Clamp(Storage.gridRenderSize, 32, 512);
		public static int ListRenderSize => Mathf.Clamp(Storage.listRenderSize, 16, 128);

		public static int GetRenderSize(ParticleThumbnailSurface surface)
		{
			return surface == ParticleThumbnailSurface.ProjectWindowList ? ListRenderSize : GridRenderSize;
		}

		public static Color BackgroundColor => Storage.backgroundColor;

		public static float CameraFov => Mathf.Clamp(Storage.cameraFov, 10f, 90f);
		public static float CameraYaw => Mathf.Clamp(Storage.cameraYaw, -180f, 180f);
		public static float CameraPitch => Mathf.Clamp(Storage.cameraPitch, -89f, 89f);
		public static float BoundsPadding => Mathf.Clamp(Storage.boundsPadding, 0f, 1f);

		public static float ScanMaxSeconds => Mathf.Clamp(Storage.scanMaxSeconds, 0.5f, 10f);
		public static float MotionPadding => Mathf.Clamp(Storage.motionPadding, 0f, 3f);
		public static float MotionRadius => Mathf.Clamp(Storage.motionRadius, 0.1f, 50f);
		public static float MotionSpeed => Mathf.Clamp(Storage.motionSpeed, 0.1f, 200f);
		public static float ThumbnailFillTarget => Mathf.Clamp(Storage.thumbnailFillTarget, 0.45f, 0.90f);
		public static bool EnableTightFraming => Storage.enableTightFraming;
		public static float ParticleFramingPercentile => Mathf.Clamp(Storage.particleFramingPercentile, 0.80f, 0.99f);

		public static int MaxRendersPerUpdate => Mathf.Max(1, Storage.maxRendersPerUpdate);
		public static float RenderBudgetMs => Mathf.Clamp(Storage.renderBudgetMs, 1f, 100f);
		public static int MemoryCacheMaxEntries => Mathf.Max(10, Storage.memoryCacheMaxEntries);
		public static bool EnablePersistentCache => Storage.enablePersistentCache;

		public static string GetPersistentSettingsToken()
		{
			string payload =
				$"{Enabled}|{DrawInProjectGrid}|{DrawInProjectList}|{GridRenderSize}|{ListRenderSize}|" +
				$"{BackgroundColor.r:F4},{BackgroundColor.g:F4},{BackgroundColor.b:F4},{BackgroundColor.a:F4}|" +
				$"{CameraFov:F4}|{CameraYaw:F4}|{CameraPitch:F4}|{BoundsPadding:F4}|" +
				$"{ScanMaxSeconds:F4}|{MotionPadding:F4}|{MotionRadius:F4}|{MotionSpeed:F4}|" +
				$"{BuildFramingSettingsFragment(EnableTightFraming, ParticleFramingPercentile, ThumbnailFillTarget)}|" +
				$"{MaxRendersPerUpdate}|{RenderBudgetMs:F4}|{MemoryCacheMaxEntries}|{EnablePersistentCache}|{SettingsTokenVersion}";
			return Hash128.Compute(payload).ToString();
		}

		internal static string BuildFramingSettingsFragment(bool enableTightFraming, float framingPercentile, float thumbnailFillTarget)
		{
			return
				$"{enableTightFraming}|{Mathf.Clamp(framingPercentile, 0.80f, 0.99f):F4}|" +
				$"{Mathf.Clamp(thumbnailFillTarget, 0.45f, 0.90f):F4}";
		}

		public static void NotifyChanged()
		{
			SettingsChanged?.Invoke();
			EditorApplication.RepaintProjectWindow();
		}
	}

	internal static class ParticleThumbnailSettingsProvider
	{
		private enum SettingsTab
		{
			Display = 0,
			Rendering = 1,
			Simulation = 2,
			Performance = 3,
		}

		private static readonly string[] MainTabTitles =
		{
			"Display",
			"Rendering",
			"Simulation",
			"Performance",
		};

		private static Vector2 SettingsScroll;
		private static SettingsTab SelectedTab = SettingsTab.Display;
		private static GUIStyle CenteredSectionHeaderStyle;

		[SettingsProvider]
		public static SettingsProvider CreateProvider()
		{
			return new SettingsProvider("Project/Particle Thumbnail & Preview/Particle Thumbnails", SettingsScope.Project)
			{
				label = "Particle Thumbnails",
				guiHandler = _ => DrawGui(),
				keywords = new System.Collections.Generic.HashSet<string>
				{
					"particle",
					"thumbnail",
					"project window",
					"preview",
				},
			};
		}

		private static void DrawGui()
		{
			ParticleThumbnailSettingsStorage storage = ParticleThumbnailSettingsStorage.instance;

			EditorGUI.BeginChangeCheck();
			DrawEnabledToggle(storage);
			SettingsScroll = EditorGUILayout.BeginScrollView(SettingsScroll, false, false);
			DrawMainTabs();
			EditorGUILayout.Space(6f);
			DrawSelectedTab(storage);
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space(8f);
			DrawBottomActionsPanel(storage);
			DrawCacheRuntimeInfo();

			if (EditorGUI.EndChangeCheck())
			{
				storage.SaveStorage();
				ParticleThumbnailSettings.NotifyChanged();
			}
		}

		private static void DrawEnabledToggle(ParticleThumbnailSettingsStorage storage)
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				storage.enabled = EditorGUILayout.Toggle(storage.enabled, GUILayout.Width(18f));
				EditorGUILayout.LabelField(
					new GUIContent("Enable Particle Thumbnails", "Turn custom particle prefab thumbnails on or off for this project."),
					EditorStyles.boldLabel);
			}
		}

		private static void DrawMainTabs()
		{
			int selected = GUILayout.Toolbar((int) SelectedTab, MainTabTitles);
			if (selected != (int) SelectedTab)
			{
				SelectedTab = (SettingsTab) selected;
				GUI.FocusControl(null);
			}
		}

		private static void DrawSelectedTab(ParticleThumbnailSettingsStorage storage)
		{
			switch (SelectedTab)
			{
				case SettingsTab.Display:
					DrawDisplayTab(storage);
					break;
				case SettingsTab.Rendering:
					DrawRenderingTab(storage);
					break;
				case SettingsTab.Simulation:
					DrawSimulationTab(storage);
					break;
				case SettingsTab.Performance:
					DrawPerformanceTab(storage);
					break;
			}
		}

		private static void DrawDisplayTab(ParticleThumbnailSettingsStorage storage)
		{
			DrawSectionCard("Draw Modes", () =>
			{
				storage.drawInProjectGrid = EditorGUILayout.Toggle("Draw In Grid", storage.drawInProjectGrid);
				storage.drawInProjectList = EditorGUILayout.Toggle("Draw In List", storage.drawInProjectList);
			});

			DrawSectionCard("Background color ", () => { storage.backgroundColor = EditorGUILayout.ColorField("Background Color", storage.backgroundColor); });
			DrawSectionCard("Thumbnail Size", () =>
			{
				storage.listRenderSize = EditorGUILayout.IntSlider("List Size (pixel)", storage.listRenderSize, 16, 128);
				storage.gridRenderSize = EditorGUILayout.IntSlider("Grid Size (pixel)", storage.gridRenderSize, 32, 512);
			});
		}

		private static void DrawRenderingTab(ParticleThumbnailSettingsStorage storage)
		{
			DrawSectionCard("Camera", () =>
			{
				storage.cameraFov = EditorGUILayout.Slider("Camera FOV", storage.cameraFov, 10f, 90f);
				storage.cameraYaw = EditorGUILayout.Slider("Camera Yaw", storage.cameraYaw, -180f, 180f);
				storage.cameraPitch = EditorGUILayout.Slider("Camera Pitch", storage.cameraPitch, -89f, 89f);
			});

			DrawSectionCard("Framing", () =>
			{
				storage.boundsPadding = EditorGUILayout.Slider("Bounds Padding", storage.boundsPadding, 0f, 1f);
				storage.thumbnailFillTarget = EditorGUILayout.Slider("Thumbnail Fill Target", storage.thumbnailFillTarget, 0.45f, 0.90f);
			});
		}

		private static void DrawSimulationTab(ParticleThumbnailSettingsStorage storage)
		{
			DrawSectionCard("Scan Window", () => { storage.scanMaxSeconds = EditorGUILayout.Slider("Scan Max Seconds", storage.scanMaxSeconds, 0.5f, 10f); });

			DrawSectionCard("Motion Assist (For moving particle)", () =>
			{
				storage.motionPadding = EditorGUILayout.Slider("Motion Padding", storage.motionPadding, 0f, 3f);
				storage.motionRadius = EditorGUILayout.Slider("Motion Radius", storage.motionRadius, 0.1f, 50f);
				storage.motionSpeed = EditorGUILayout.Slider("Motion Speed", storage.motionSpeed, 0.1f, 200f);
			});
		}

		private static void DrawPerformanceTab(ParticleThumbnailSettingsStorage storage)
		{
			DrawSectionCard("Generation Budget", () =>
			{
				storage.maxRendersPerUpdate = EditorGUILayout.IntSlider("Max Renders / Update", storage.maxRendersPerUpdate, 1, 16);
				storage.renderBudgetMs = EditorGUILayout.Slider("Render Budget (ms)", storage.renderBudgetMs, 1f, 100f);
			});

			DrawSectionCard("Cache Limits", () =>
			{
				storage.memoryCacheMaxEntries = EditorGUILayout.IntSlider("Memory Cache Entries", storage.memoryCacheMaxEntries, 10, 1000);
				storage.enablePersistentCache = EditorGUILayout.Toggle("Enable Persistent Cache", storage.enablePersistentCache);
			});
		}

		private static void DrawBottomActionsPanel(ParticleThumbnailSettingsStorage storage)
		{
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Reset To Defaults", GUILayout.Height(28f)) &&
					    EditorUtility.DisplayDialog(
						    "Reset Particle Thumbnail Settings",
						    "Reset all particle thumbnail settings back to default values?",
						    "Reset",
						    "Cancel"))
					{
						storage.ResetToDefaults();
						storage.SaveStorage();
						ParticleThumbnailSettings.NotifyChanged();
						ParticleThumbnailService.ClearMemoryCache();
						GUIUtility.ExitGUI();
					}

					if (GUILayout.Button("Rebuild Visible", GUILayout.Height(28f)))
						ParticleThumbnailService.RebuildVisibleThumbnails();
				}

				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Clear Memory Cache", GUILayout.Height(28f)))
						ParticleThumbnailService.ClearMemoryCache();

					if (GUILayout.Button("Clear Disk Cache", GUILayout.Height(28f)))
						ParticleThumbnailService.ClearPersistentCache();
				}

				EditorGUILayout.Space(4f);
				using (new EditorGUI.DisabledScope(ParticleThumbnailService.IsGenerateAllInProgress))
				{
					if (GUILayout.Button("Generate All Thumbnails In Project", GUILayout.Height(32f)))
						ParticleThumbnailService.GenerateAllThumbnailsInProject();
				}
			}
		}

		private static void DrawCacheRuntimeInfo()
		{
			ParticleThumbnailService.CacheStats stats = ParticleThumbnailService.GetCacheStats();
			string memorySize = FormatBytes(stats.MemoryCacheBytes);
			string diskSize = FormatBytes(stats.DiskCacheBytes);

			EditorGUILayout.Space(6f);
			EditorGUILayout.HelpBox(
				$"Memory: {stats.TotalEntries} entries ({memorySize})  |  Disk: {stats.PersistentEntryCount} files ({diskSize})  |  Generating: {stats.GeneratingCount}  |  Failed: {stats.FailedCount}  |  Queue: {stats.QueueDepth}",
				MessageType.None);
		}

		private static string FormatBytes(long bytes)
		{
			if (bytes < 1048576L)
				return $"{bytes / 1024f:F0} KB";

			return $"{bytes / 1048576f:F1} MB";
		}

		private static void DrawSectionCard(string title, Action drawContent)
		{
			EditorGUILayout.Space(6f);
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				DrawSectionHeader(title);
				drawContent?.Invoke();
			}
		}

		private static void DrawSectionHeader(string title)
		{
			if (CenteredSectionHeaderStyle == null)
			{
				CenteredSectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
				{
					alignment = TextAnchor.MiddleCenter
				};
			}

			Rect rect = EditorGUILayout.GetControlRect(false, 20f);
			EditorGUI.LabelField(rect, title, CenteredSectionHeaderStyle);

			Rect lineRect = new Rect(rect.x, rect.yMax + 1f, rect.width, 1f);
			EditorGUI.DrawRect(lineRect, new Color(0.30f, 0.30f, 0.30f, 1f));
			EditorGUILayout.Space(4f);
		}
	}
}