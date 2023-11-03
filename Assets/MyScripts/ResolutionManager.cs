using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
	public static ResolutionManager instance;

	public int deviceWidth;		// ��� �ʺ� ����
	public int deviceHeight;	// ��� ���� ����

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Screen.orientation = ScreenOrientation.LandscapeRight;
			SetResolution();
		}
		else
		{
			Destroy(gameObject);
		}

	}

	void Start()
    {
		
	}

    void Update()
    {
	}

	public void SetResolution()
	{
		int setWidth = 1920; // ����� ���� �ʺ�
		int setHeight = 1080; // ����� ���� ����

		deviceWidth = Screen.width; // ��� �ʺ� ����
		deviceHeight = Screen.height; // ��� ���� ����

		Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

		if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
		{
			float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
			Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
		}
		else // ������ �ػ� �� �� ū ���
		{
			float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
			Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
		}

	}
}
