﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	public static MapManager instance;
	void Awake()
	{
		instance = this;
	}
	public string currentlevel;
	public string nextLevel;
}
