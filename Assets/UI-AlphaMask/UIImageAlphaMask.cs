using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// uGUI ImageにAlphaMaskをかけるコンポーネント
/// </summary>
/// 以下の制限がある。
/// - CanvasのRenderModeがScreenSpaceCamera/WorldSpaceであること
/// - CanvasのworldCameraがOtrhographicであること
/// - Maskの回転、タイリング、9スライスは使用できない
/// - Mask画像はWrapModeをClampに設定し、画像端の1ピクセルをアルファ0で描くこと
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UIImageAlphaMask : MonoBehaviour
{
	private Image m_MaskImage;
	private Canvas m_Canvas;
	private RectTransform m_RectTransform;
	private Image[] m_TargetImageList;
	private Material m_Material;

	private Material Material{
		get{
			if (m_Material == null) {
				m_Material = new Material (Shader.Find ("Hidden/UI/AlphaMask"));
			}
			return m_Material;
		}
	}

	private Image MaskImage{
		get{
			if (m_MaskImage == null) {
				m_MaskImage = GetComponent<Image> ();
			}
			return m_MaskImage;
		}
	}

	private RectTransform SelfRect{
		get{
			if (m_RectTransform == null) {
				m_RectTransform = GetComponent<RectTransform> ();
			}
			return m_RectTransform;
		}
	}

	private Canvas RootCanvas{
		get{
			if (m_Canvas == null) {
				var canvas = GetComponentInParent<Canvas> ();
				if (canvas != null) {
					if (canvas.rootCanvas != null) {
						m_Canvas = canvas.rootCanvas;
					} else {
						m_Canvas = canvas;
					}
				}
			}
			return m_Canvas;
		}
	}


	private void SetMaterial(Image[] imageList, Material material)
	{
		if (imageList == null) {
			return;
		}

		for (int i = 0; i < imageList.Length; i++) {
			var image = imageList [i];
			image.material = material;
		}
	}

	private void Update()
	{
		if (!IsValid ()) {
			return;
		}

		if (m_TargetImageList == null || m_TargetImageList.Length <= 0) {
			m_TargetImageList = GetComponentsInChildren<Image> (true);
			m_TargetImageList = m_TargetImageList.Where (o => o.transform != this.transform).ToArray ();
			SetMaterial (m_TargetImageList, Material);
		}

		SetProperties ();
	}

	private bool IsValid()
	{
		if (RootCanvas == null) {
			return false;
		}

		if (RootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) {
			return false;
		}

		if (MaskImage.sprite == null) {
			return false;
		}

		return true;
	}

	private void SetProperties()
	{
		var camera = GetCamera ();
		if (camera == null) {
			return;
		}

		var matrix = CalculateMatrix (camera);

		Material.SetTexture ("_MaskTex", MaskImage.sprite.texture);
		Material.SetMatrix ("_MaskMatrix", matrix);
	}

	private Camera GetCamera()
	{
		return RootCanvas.worldCamera;
	}

	private Matrix4x4 CalculateMatrix(Camera camera)
	{
		var rect = CalcurateViewportRect (camera);

		Matrix4x4 result = Matrix4x4.identity;

		var halfSize = rect.size * 0.5f;
		result = Matrix4x4.Ortho (
			rect.center.x - halfSize.x,
			rect.center.x + halfSize.x,
			rect.center.y - halfSize.y,
			rect.center.y + halfSize.y,
			camera.nearClipPlane,
			camera.farClipPlane
		);
		result = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f) * GL.GetGPUProjectionMatrix (result, false) * camera.worldToCameraMatrix;
		return result;
	}

	private Rect CalcurateViewportRect(Camera camera)
	{
		var corners = new Vector3[ 4 ];

		SelfRect.GetWorldCorners( corners );

		var p1 = RectTransformUtility.WorldToScreenPoint( camera, corners[ 1 ] );
		var p3 = RectTransformUtility.WorldToScreenPoint( camera, corners[ 3 ] );

		var x = p1.x - (Screen.width * 0.5f);
		var y = p3.y - (Screen.height * 0.5f);
		var width = p3.x - p1.x;
		var height = p1.y - p3.y;

		x /= (Screen.width * 0.5f);
		width /= (Screen.width * 0.5f);
		y /= (Screen.height * 0.5f);
		height /= (Screen.height * 0.5f);

		var size = camera.orthographicSize;
		var aspect = Screen.width / (float)Screen.height;
		x *= size * aspect;
		width *= size * aspect;
		y *= size;
		height *= size;

		var rect = new Rect(){
			x = x,
			width = width,
			y = y,
			height = height
		};
		return rect;
	}
}
