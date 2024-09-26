using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : Photon.MonoBehaviour, IDestroyable, IIShootableThrowable, IShootable
{
	[Serializable]
	public class BonusSounds
	{
		public AudioClip lifeHeart;

		public AudioClip ammo;

		public AudioClip jump;

		public AudioClip grounded;
	}

	[Serializable]
	public class PlayerInfo
	{
		public string name = "Player";

		public float hitPoints;

		public Sprite playerIcon;

		public int max_hp = 70;

		public int deathCount;

		public int killAssistsCount;

		public int score;

		public void AddHP(int val)
		{
			hitPoints += val;
			if (hitPoints > (float)max_hp)
			{
				hitPoints = max_hp;
			}
		}
	}

	private struct BufferRecord
	{
		public Vector3 pos;

		public double time;

		public float spineAngle;

		public float eulerY;

		public BufferRecord(Vector3 pos, double time, float spineEulers, float eulerY)
		{
			this.pos = pos;
			this.time = time;
			spineAngle = spineEulers;
			this.eulerY = eulerY;
		}
	}

	public float lastNetworkActTime;

	[HideInInspector]
	public Vector2 moveNormalDir = Vector2.zero;

	[HideInInspector]
	public float vertVelocity;

	public CharacterController characterController;

	public float gravity = 20f;

	public float SpeedValue;

	private float currentSpeed;

	public float jumpSpeed = 11f;

	public float MAX_NEGATIVE_SPEED_VALUE = 30f;

	[HideInInspector]
	public int fragsCount;

	[SerializeField]
	private GameObject shadowPlane;

	public BonusSounds bonusSounds;

	public AudioSource BonusAudioSource;

	public AudioSource footstepsAudio;

	public Action<UnityEngine.Object> PlayerCrashed;

	public Action<UnityEngine.Object> PlayerRespawned;

	public Action<Vehicle> PlayerSitCar;

	public Action<Vehicle> PlayerLeaveCar;

	public Action<UnityEngine.Object, int> DamagedBySomeone;

	public static Action<CharacterMotor> ScoreChanged;

	public TPSCamSettings[] camFollowSettings;

	public TPSCamSettings[] defaultCamFollowSettings;

	public PlayerInfo playerInfo;

	public PlayerWeaponManager playerWeaponManager;

	public CharacterAnimation characterAnimation;

	public PlayerClothingManager playerClothingManager;

	private CharacterAudioController characterAudioController;

	public bool IsAttacking;

	public float timeInAir;

	public Transform bodyContainer;

	public bool isStandingOnBall;

	public bool isJetFlying;

	public bool isInPauseMode;

	public float lastActTime;

	public TeamID myTeam;

	public GameObject HPBarPrefab;

	public bool isNowShooting;

	private int lastDamagedMeViewId;

	public int lastKillerId;

	private List<int> myDamagers = new List<int>();

	public Vehicle myCar;

	public int mySittingCarViewId = -1;

	public PlayerInventaryManager playerInventaryManager;

	private bool isClone;

	public bool isInCar;

	[SerializeField]
	private GameObject ghostPrefab;

	[SerializeField]
	private Bodyguard myBodyguard;

	public float lastAtGunPointTime;

	private List<BufferRecord> BufferedPositions = new List<BufferRecord>();

	public Transform hudPivot;

	private Vector3 recivedPlayerPos;

	private float recivedPlayerRotY;

	private float cloneSpineEules;

	private Vector3 bufMoveDir;

	private Quaternion q1 = Quaternion.identity;

	private Quaternion q2 = Quaternion.identity;

	private int index;

	private float smt;

	private float prevVelocityX;

	private float prevVelocityY;

	public JetPack myJetPack;

	private Vector3 TargetAndLookPoint;

	private bool isInRotatingState;

	[HideInInspector]
	public float bodyVertAngle;

	[HideInInspector]
	public float lastShootTime;

	[SerializeField]
	private AudioClip[] killSXs;

	private GameObject killFXgo;

	private RaycastHit hitinfo;

	private Ray ray = default(Ray);

	private Vector3 _lastPosBeforePause;

	private bool isPaused;

	[SerializeField]
	private LayerMask shadowPlaneLayers;

	[SerializeField]
	private Transform grenagePivot;

	private byte bufGrenadeType;

	private ArmGrenade grenade;

	public Transform FPSCameraPosition => playerWeaponManager.CurrentWeapon.fpsCameraPivot;

	public float HP => playerInfo.hitPoints;

	public Bodyguard MyBodyguard
	{
		get
		{
			if (myBodyguard == null)
			{
				return null;
			}
			return myBodyguard;
		}
	}

	public int ViewId => base.photonView.viewID;

	public CharacterMotor LastKiller
	{
		get
		{
			PhotonView photonView = PhotonView.Find(lastKillerId);
			if (photonView != null)
			{
				return photonView.GetComponent<CharacterMotor>();
			}
			return null;
		}
	}

	private void Awake()
	{
		playerWeaponManager = GetComponent<PlayerWeaponManager>();
		playerInventaryManager = GetComponent<PlayerInventaryManager>();
		playerClothingManager = GetComponent<PlayerClothingManager>();
		characterAudioController = GetComponent<CharacterAudioController>();
		float globalScaleKoeff = GameController.globalScaleKoeff;
		base.transform.localScale = new Vector3(globalScaleKoeff, globalScaleKoeff, globalScaleKoeff);
		CheckOptimization();
		camFollowSettings = GetComponent<TPSCamSettingsList>().camSettingsList;
		defaultCamFollowSettings = new TPSCamSettings[camFollowSettings.Length];
		for (int i = 0; i < camFollowSettings.Length; i++)
		{
			defaultCamFollowSettings[i] = camFollowSettings[i];
		}
	}

	private IEnumerator Start()
	{
		playerInfo.hitPoints = playerInfo.max_hp;
		if (!base.photonView.isMine)
		{
			base.transform.SetLocalEulerY(-100f);
			isClone = true;
			while (GameController.instance == null)
			{
				yield return null;
			}
			EnableShadow(enable: false);
		}
		else
		{
			GetComponent<ChatController>().AddSimpleMessage(StorageController.instance.PlayerName + " joined game");
			PhotonNetwork.player.NickName = StorageController.instance.PlayerName;
		}
		if (base.photonView.isMine)
		{
			HipPointsWidget.instance.UpdateHP(playerInfo.hitPoints);
		}
		GameWindow.instance.HideWaitingOtherPlayersPanel();
		if (base.photonView.isMine)
		{
			HipPointsWidget.instance.UpdateHP(playerInfo.hitPoints);
		}
		yield return null;
		InitVars();
		ApplySkinClothing();
		GameController.instance.OnPlayerConnected(base.gameObject);
		footstepsAudio.Stop();
	}

	public void OnNewPlayerJoined()
	{
		if (base.photonView.isMine)
		{
			PhotonNetwork.RPC(base.photonView, "UpdStateOnNew", PhotonTargets.Others, false, HP, fragsCount, playerInfo.score, mySittingCarViewId, playerWeaponManager.CurrentWeapon.gunId);
		}
	}

	[PunRPC]
	private void UpdStateOnNew(float newHP, int frags, int score, int myCurrentSittingCar, string gunId)
	{
		playerInfo.hitPoints = newHP;
		SetScore(score);
		fragsCount = frags;
		if (myCurrentSittingCar != -1)
		{
			OnEnterCarRPC(myCurrentSittingCar);
		}
		playerWeaponManager.SelectWeapon(gunId);
	}

	private void InitVars()
	{
		lastActTime = Time.time;
		currentSpeed = SpeedValue;
	}

	private void ApplySkinClothing()
	{
		if (!(base.photonView == null) && base.photonView.instantiationData != null)
		{
			playerInfo.name = (string)base.photonView.instantiationData[0];
			string text = base.photonView.instantiationData[1].ToString();
			if (!string.IsNullOrEmpty(text))
			{
				myTeam = (TeamID)int.Parse(text);
				GetComponent<PlayerClothingManager>().SetUpBody(myTeam == TeamID.TeamA, base.photonView.viewID);
			}
			base.gameObject.name = "Player_" + myTeam + "__" + playerInfo.name;
			if (base.photonView.isMine)
			{
				base.gameObject.name = "-------OurPlayer-------" + myTeam + "_" + playerInfo.name;
			}
			if (!base.photonView.isMine)
			{
				string hatId = (string)base.photonView.instantiationData[2];
				playerClothingManager.WearHat(hatId);
				hatId = (string)base.photonView.instantiationData[3];
				playerClothingManager.WearEyes(hatId);
				hatId = (string)base.photonView.instantiationData[4];
				playerClothingManager.WearSmile(hatId);
				hatId = (string)base.photonView.instantiationData[5];
				playerClothingManager.WearShoes(hatId);
			}
			else
			{
				playerClothingManager.HideHead();
			}
		}
	}

	private void Update()
	{
		if (!isPaused && base.photonView.isMine)
		{
			CheckStateTransitions();
			CalcVerticalSpeed();
			Move();
			HighlightEnemies();
			if (isClone)
			{
				OnBecameSceneObject();
			}
		}
	}

	private void LateUpdate()
	{
		if (!base.photonView.isMine)
		{
			UpdateClonePosition();
		}
		if (!isPaused)
		{
			RaycastShadowPos();
		}
	}

	public virtual void ApplyDamage(float val, BodyPart bodyPart, int fromWhom)
	{
		if (!(BattleRoyaleLobbyWaitingPanel.instance != null) || !BattleRoyaleLobbyWaitingPanel.instance.isWaiting)
		{
			ApplyDamage(val, fromWhom);
		}
	}

	public virtual void ApplyDamage(float val, int fromWhom)
	{
		if (isInCar && myCar.vehicleType != VehicleType.Moto)
		{
			return;
		}
		if (GameController.gameConfigData.gameMode == GameMode.TeamFight)
		{
			PhotonView photonView = PhotonView.Find(fromWhom);
			if (photonView != null)
			{
				CharacterMotor component = photonView.GetComponent<CharacterMotor>();
				if (component != null && component.myTeam == myTeam)
				{
					return;
				}
			}
		}
		if (val >= playerInfo.hitPoints)
		{
			PlayKillFX();
		}
		PhotonNetwork.RPC(base.photonView, "DamageRPC", PhotonTargets.All, false, val, fromWhom);
	}

	[PunRPC]
	public void DamageRPC(float dmg, int fromWhom)
	{
		if (!myDamagers.Contains(fromWhom))
		{
			myDamagers.Add(fromWhom);
		}
		lastDamagedMeViewId = fromWhom;
		if (base.photonView.isMine)
		{
			if (playerInfo.hitPoints == 0f)
			{
				return;
			}
			playerInfo.hitPoints -= dmg;
			if (playerInfo.hitPoints < 0f)
			{
				playerInfo.hitPoints = 0f;
			}
			if (DamagedBySomeone != null)
			{
				DamagedBySomeone(this, fromWhom);
			}
		}
		if (base.photonView.isMine)
		{
			PhotonNetwork.RPC(base.photonView, "UpdateHP", PhotonTargets.All, false, playerInfo.hitPoints);
		}
	}

	[PunRPC]
	public void UpdateHP(float newHP)
	{
		playerInfo.hitPoints = newHP;
		if (playerInfo.hitPoints == 0f)
		{
			lastKillerId = lastDamagedMeViewId;
			OnDie();
		}
		if (base.photonView.isMine)
		{
			HipPointsWidget.instance.UpdateHP(playerInfo.hitPoints);
		}
	}

	public virtual void ApplyHeal(float val)
	{
		if (base.photonView.isMine)
		{
			playerInfo.hitPoints += val;
			if (playerInfo.hitPoints > (float)playerInfo.max_hp)
			{
				playerInfo.hitPoints = playerInfo.max_hp;
			}
			PhotonNetwork.RPC(base.photonView, "UpdateHP", PhotonTargets.All, false, playerInfo.hitPoints);
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			Vector3 eulerAngles = base.transform.eulerAngles;
			stream.SendNext(eulerAngles.y);
			stream.SendNext(moveNormalDir);
			Vector3 eulerAngles2 = characterAnimation.topBody.eulerAngles;
			stream.SendNext(eulerAngles2.x);
			return;
		}
		recivedPlayerPos = (Vector3)stream.ReceiveNext();
		recivedPlayerRotY = (float)stream.ReceiveNext();
		moveNormalDir = (Vector2)stream.ReceiveNext();
		cloneSpineEules = (float)stream.ReceiveNext();
		BufferedPositions.Add(new BufferRecord(recivedPlayerPos, PhotonNetwork.time, cloneSpineEules, recivedPlayerRotY));
		if (BufferedPositions.Count > 4)
		{
			BufferedPositions.RemoveAt(0);
		}
	}

	private void Move()
	{
		if (IsAlive() && !isInCar)
		{
			bufMoveDir = base.transform.TransformDirection(new Vector3(moveNormalDir.x, 0f, moveNormalDir.y));
			bufMoveDir *= currentSpeed;
			bufMoveDir.y = vertVelocity;
			characterController.Move(bufMoveDir * Time.deltaTime);
		}
	}

	private void UpdateClonePosition()
	{
		if (isInCar)
		{
			return;
		}
		if (BufferedPositions.Count >= 2)
		{
			index = BufferedPositions.FindLastIndex((BufferRecord item) => item.time < myPhotonTime());
			if (index != -1)
			{
				if (index == BufferedPositions.Count - 1)
				{
					Transform transform = base.transform;
					BufferRecord bufferRecord = BufferedPositions[index];
					transform.position = bufferRecord.pos;
					Transform transform2 = base.transform;
					BufferRecord bufferRecord2 = BufferedPositions[index];
					transform2.SetEulerY(bufferRecord2.eulerY);
					CharacterAnimation obj = characterAnimation;
					BufferRecord bufferRecord3 = BufferedPositions[index];
					obj.SetSpineEulers(bufferRecord3.spineAngle);
					return;
				}
				double num = myPhotonTime();
				BufferRecord bufferRecord4 = BufferedPositions[index];
				double num2 = num - bufferRecord4.time;
				BufferRecord bufferRecord5 = BufferedPositions[index + 1];
				double time = bufferRecord5.time;
				BufferRecord bufferRecord6 = BufferedPositions[index];
				smt = (float)(num2 / (time - bufferRecord6.time));
				Transform transform3 = base.transform;
				BufferRecord bufferRecord7 = BufferedPositions[index];
				Vector3 pos = bufferRecord7.pos;
				BufferRecord bufferRecord8 = BufferedPositions[index + 1];
				transform3.position = Vector3.Lerp(pos, bufferRecord8.pos, smt);
				Vector3 eulerAngles = base.transform.eulerAngles;
				float x = eulerAngles.x;
				BufferRecord bufferRecord9 = BufferedPositions[index];
				float eulerY = bufferRecord9.eulerY;
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				q1 = Quaternion.Euler(x, eulerY, eulerAngles2.z);
				Vector3 eulerAngles3 = base.transform.eulerAngles;
				float x2 = eulerAngles3.x;
				BufferRecord bufferRecord10 = BufferedPositions[index + 1];
				float eulerY2 = bufferRecord10.eulerY;
				Vector3 eulerAngles4 = base.transform.eulerAngles;
				q2 = Quaternion.Euler(x2, eulerY2, eulerAngles4.z);
				base.transform.rotation = Quaternion.Slerp(q1, q2, smt);
				CharacterAnimation obj2 = characterAnimation;
				BufferRecord bufferRecord11 = BufferedPositions[index];
				float spineAngle = bufferRecord11.spineAngle;
				BufferRecord bufferRecord12 = BufferedPositions[index + 1];
				obj2.SetSpineEulers(Mathf.LerpAngle(spineAngle, bufferRecord12.spineAngle, smt));
			}
		}
		else if (BufferedPositions.Count == 1)
		{
			Transform transform4 = base.transform;
			BufferRecord bufferRecord13 = BufferedPositions[0];
			transform4.position = bufferRecord13.pos;
			Transform transform5 = base.transform;
			BufferRecord bufferRecord14 = BufferedPositions[0];
			transform5.SetEulerY(bufferRecord14.eulerY);
			CharacterAnimation obj3 = characterAnimation;
			BufferRecord bufferRecord15 = BufferedPositions[0];
			obj3.SetSpineEulers(bufferRecord15.spineAngle);
		}
	}

	private double myPhotonTime()
	{
		return PhotonNetwork.time - 0.20000000298023224;
	}

	private bool lastBuf(BufferRecord arg)
	{
		return arg.time < myPhotonTime();
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.CompareTag("CarSensTrigger") || other.CompareTag("MotoSensTrigger") || other.CompareTag("HeliSensTrigger") || other.CompareTag("HorseSensTrigger") || other.CompareTag("PlaneSensTrigger")) && base.photonView.isMine && GameWindow.instance != null)
		{
			GameWindow.instance.ShowGetInCarBtn(show: false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ammo"))
		{
			if (!base.photonView.isMine)
			{
				playerWeaponManager.CurrentWeapon.OnFindAmmo();
				UnityEngine.Object.Destroy(other.gameObject);
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
			}
			else if (playerWeaponManager.CurrentWeapon.OnFindAmmo())
			{
				UnityEngine.Object.Destroy(other.gameObject);
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
			}
		}
		else if (other.CompareTag("LootGun"))
		{
			if (base.photonView.isMine)
			{
				playerWeaponManager.AddWeapon(other.name);
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
				UnityEngine.Object.Destroy(other.gameObject);
			}
			else
			{
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
				UnityEngine.Object.Destroy(other.gameObject);
			}
		}
		else if (other.CompareTag("Grenade"))
		{
			if (base.photonView.isMine)
			{
				if (playerWeaponManager.FindedGrenade(1, BulletType.Grenade))
				{
					BonusAudioSource.PlayOneShot(bonusSounds.ammo);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else
			{
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
				UnityEngine.Object.Destroy(other.gameObject);
			}
		}
		else if (other.CompareTag("Life"))
		{
			if (!base.photonView.isMine)
			{
				UnityEngine.Object.Destroy(other.gameObject);
			}
			else if ((float)playerInfo.max_hp - playerInfo.hitPoints > 0f)
			{
				UnityEngine.Object.Destroy(other.gameObject);
				BonusAudioSource.PlayOneShot(bonusSounds.lifeHeart);
				GetComponent<DamageReciver2>().Heal(20f);
			}
		}
		else if (other.CompareTag("SmokeGrenade"))
		{
			if (base.photonView.isMine)
			{
				if (playerWeaponManager.FindedGrenade(1, BulletType.SmokeGrenade))
				{
					BonusAudioSource.PlayOneShot(bonusSounds.ammo);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else
			{
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
				UnityEngine.Object.Destroy(other.gameObject);
			}
		}
		else if (other.CompareTag("MolotovGrenade"))
		{
			if (base.photonView.isMine)
			{
				if (playerWeaponManager.FindedGrenade(1, BulletType.MolotovGrenade))
				{
					BonusAudioSource.PlayOneShot(bonusSounds.ammo);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else
			{
				BonusAudioSource.PlayOneShot(bonusSounds.ammo);
				UnityEngine.Object.Destroy(other.gameObject);
			}
		}
		else if (other.CompareTag("CarSensTrigger"))
		{
			if (base.photonView.isMine && other.GetComponentInParent<CarController>().AreFreeForSitting)
			{
				GameWindow.instance.ShowGetInCarBtn(show: true);
				myCar = other.GetComponentInParent<Vehicle>();
			}
		}
		else if (other.CompareTag("MotoSensTrigger"))
		{
			if (base.photonView.isMine && other.GetComponentInParent<MotoController>().AreFreeForSitting)
			{
				GameWindow.instance.ShowGetInCarBtn(show: true);
				myCar = other.GetComponentInParent<Vehicle>();
			}
		}
		else if (other.CompareTag("HeliSensTrigger"))
		{
			if (base.photonView.isMine && other.GetComponentInParent<HelicopterController>().AreFreeForSitting)
			{
				GameWindow.instance.ShowGetInCarBtn(show: true);
				myCar = other.GetComponentInParent<Vehicle>();
			}
		}
		else if (other.CompareTag("HorseSensTrigger"))
		{
			if (base.photonView.isMine && other.GetComponentInParent<HorseController>().AreFreeForSitting)
			{
				GameWindow.instance.ShowGetInCarBtn(show: true);
				myCar = other.GetComponentInParent<Vehicle>();
			}
		}
		else if (other.CompareTag("PlaneSensTrigger") && base.photonView.isMine && other.GetComponentInParent<PlaneController>().AreFreeForSitting)
		{
			GameWindow.instance.ShowGetInCarBtn(show: true);
			myCar = other.GetComponentInParent<Vehicle>();
		}
	}

	private void CheckStateTransitions()
	{
		if ((prevVelocityX == 0f && moveNormalDir.x != 0f) || (prevVelocityY == 0f && moveNormalDir.y != 0f))
		{
			OnRunStart();
		}
		prevVelocityX = moveNormalDir.x;
		prevVelocityY = moveNormalDir.y;
	}

	public void OnStartJetFlying()
	{
		OnStartJetFlyingRPC();
		PhotonNetwork.RPC(base.photonView, "OnStartJetFlyingRPC", PhotonTargets.Others, false);
	}

	[PunRPC]
	private void OnStartJetFlyingRPC()
	{
		isJetFlying = true;
		characterAnimation.OnStartFlyingOnJetpack();
		myJetPack.StartEngine();
	}

	public void OnStopJetFlying()
	{
		OnStopJetFlyingRPC();
		PhotonNetwork.RPC(base.photonView, "OnStopJetFlyingRPC", PhotonTargets.Others, false);
	}

	[PunRPC]
	private void OnStopJetFlyingRPC()
	{
		isJetFlying = false;
		myJetPack.StopEngine();
	}

	private void OnRunStart()
	{
		StartCoroutine(RotateToTargetPoint(null));
	}

	private void CalcVerticalSpeed()
	{
		if (characterController.isGrounded)
		{
			if (timeInAir > 0.5f)
			{
				characterAnimation.OnGround();
				footstepsAudio.PlayOneShot(bonusSounds.grounded);
			}
			timeInAir = 0f;
			if (vertVelocity < -1f && !isJetFlying)
			{
				vertVelocity = 0f;
			}
		}
		else
		{
			timeInAir += Time.deltaTime;
		}
		if (!isJetFlying)
		{
			vertVelocity -= gravity * Time.deltaTime;
			if (vertVelocity < 0f - MAX_NEGATIVE_SPEED_VALUE)
			{
				vertVelocity = 0f - MAX_NEGATIVE_SPEED_VALUE;
			}
		}
	}

	private IEnumerator RotateToTargetPoint(Action actionOnRotationComplete)
	{
		if (GameController.instance.cameraMode != 0)
		{
			Vector3 dir = TargetAndLookPoint - base.transform.position;
			Quaternion targetQuaternion = Quaternion.LookRotation(dir);
			Vector3 eulerAngles = base.transform.eulerAngles;
			float y = eulerAngles.y;
			Vector3 eulerAngles2 = targetQuaternion.eulerAngles;
			float horizontalDeltaAngle = Mathf.Abs(y - eulerAngles2.y);
			while (horizontalDeltaAngle > 1f)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetQuaternion, 600f * Time.deltaTime);
				Transform transform = base.transform;
				Vector3 eulerAngles3 = base.transform.eulerAngles;
				transform.eulerAngles = new Vector3(0f, eulerAngles3.y, 0f);
				Vector3 eulerAngles4 = base.transform.eulerAngles;
				float y2 = eulerAngles4.y;
				Vector3 eulerAngles5 = targetQuaternion.eulerAngles;
				horizontalDeltaAngle = Mathf.Abs(y2 - eulerAngles5.y);
				isInRotatingState = true;
				yield return null;
				isInRotatingState = false;
			}
			isInRotatingState = false;
			actionOnRotationComplete?.Invoke();
		}
	}

	public void SetGunTargetPoint(Vector3 v)
	{
	}

	private float HorizontalDeltaAngleToTarget()
	{
		Vector3 forward = TargetAndLookPoint - base.transform.position;
		Quaternion quaternion = Quaternion.LookRotation(forward);
		Vector3 eulerAngles = base.transform.eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = quaternion.eulerAngles;
		return Mathf.Abs(y - eulerAngles2.y);
	}

	public void SetTargetAndLookPoint(Vector3 point, CamMode camMode)
	{
		TargetAndLookPoint = point;
		if (characterController.velocity.sqrMagnitude > 0f || isNowShooting || camMode == CamMode.TPSnear)
		{
			Vector3 forward = TargetAndLookPoint - base.transform.position;
			Quaternion rotation = Quaternion.LookRotation(forward);
			base.transform.rotation = rotation;
			Transform transform = base.transform;
			Vector3 eulerAngles = base.transform.eulerAngles;
			transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
		}
	}

	public bool CanJump()
	{
		if (characterController.isGrounded)
		{
			return true;
		}
		return false;
	}

	public bool IsGrounded()
	{
		return characterController.isGrounded;
	}

	public bool IsAlive()
	{
		return playerInfo.hitPoints > 0f;
	}

	public void StartShoot(float holdStrength = 0f)
	{
		if (isNowShooting || Time.time - lastShootTime < playerWeaponManager.CurrentWeapon.shootDuration || playerWeaponManager.CurrentWeapon.isReloading)
		{
			return;
		}
		if (GameController.instance.cameraMode == CameraMode.TPS)
		{
			if (isInRotatingState)
			{
				return;
			}
			if (HorizontalDeltaAngleToTarget() > 1f)
			{
				StartCoroutine(RotateToTargetPoint(delegate
				{
					StartShoot(holdStrength);
				}));
				return;
			}
			isNowShooting = true;
			if (base.photonView.isMine)
			{
				StartShootRPC(holdStrength);
				PhotonNetwork.RPC(base.photonView, "StartShootRPC", PhotonTargets.Others, false, holdStrength);
			}
		}
		else
		{
			isNowShooting = true;
			if (base.photonView.isMine)
			{
				StartShootRPC(holdStrength);
				PhotonNetwork.RPC(base.photonView, "StartShootRPC", PhotonTargets.Others, false, holdStrength);
			}
		}
	}

	[PunRPC]
	public void StartShootRPC(float strength = 0f)
	{
		if (playerWeaponManager != null && playerWeaponManager.CurrentWeapon != null)
		{
			lastShootTime = Time.time;
			playerWeaponManager.CurrentWeapon.StartShooting(strength);
			characterAnimation.Shoot();
		}
	}

	public void StopShooting()
	{
		if (base.photonView.isMine)
		{
			StopShootingRPC();
			PhotonNetwork.RPC(base.photonView, "StopShootingRPC", PhotonTargets.Others, false);
		}
	}

	[PunRPC]
	public void StopShootingRPC()
	{
		isNowShooting = false;
		characterAnimation.UnShoot();
		if (playerWeaponManager != null && playerWeaponManager.CurrentWeapon != null)
		{
			playerWeaponManager.CurrentWeapon.StopShooting();
		}
	}

	public void PushBullet()
	{
		if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.CurrentWeapon.isActiveAndEnabled)
		{
			playerWeaponManager.CurrentWeapon.PushBullet();
		}
	}

	public void ApplyMeleeAttack()
	{
		if (playerWeaponManager.CurrentWeapon != null && playerWeaponManager.CurrentWeapon.isActiveAndEnabled)
		{
			playerWeaponManager.ApplyMeleeAttack();
		}
	}

	public void PushBulletForRPC(Vector3 pos, Vector3 rot, float timeSinceGameStart)
	{
		PhotonNetwork.RPC(base.photonView, "PushBulletRPC", PhotonTargets.All, false, pos, rot, timeSinceGameStart);
	}

	[PunRPC]
	public void PushBulletRPC(Vector3 pos, Vector3 rot, float timeSinceGameStart)
	{
		if (!(playerWeaponManager.CurrentWeapon == null))
		{
			playerWeaponManager.CurrentWeapon.ShootFromRPC(pos, rot, timeSinceGameStart);
		}
	}

	public void ThrowBulletForRPC(Vector3 pos, Vector3 rot, float strength)
	{
		PhotonNetwork.RPC(base.photonView, "ThrowBulletRPC", PhotonTargets.All, false, pos, rot, strength);
	}

	[PunRPC]
	public void ThrowBulletRPC(Vector3 pos, Vector3 rot, float strength)
	{
		if (playerWeaponManager != null && playerWeaponManager.CurrentWeapon != null)
		{
			playerWeaponManager.CurrentWeapon.ThrowFromRPC(pos, rot, strength);
		}
	}

	public void Jump()
	{
		StartCoroutine(JumpCRT());
	}

	private IEnumerator JumpCRT()
	{
		int i = 0;
		while (true)
		{
			if (i < 10)
			{
				if (characterController.isGrounded)
				{
					break;
				}
				yield return null;
				i++;
				continue;
			}
			yield break;
		}
		vertVelocity = jumpSpeed;
		characterAnimation.Jump();
		footstepsAudio.PlayOneShot(bonusSounds.jump);
	}

	private void WriteChatFragMsg(int killerViewId, int targetViewId)
	{
		PhotonNetwork.RPC(base.photonView, "WriteChatFragMsgR", PhotonTargets.All, false, killerViewId, targetViewId);
	}

	[PunRPC]
	private void WriteChatFragMsgR(int killerViewId, int targetViewId)
	{
		PhotonView photonView = PhotonView.Find(killerViewId);
		PhotonView photonView2 = PhotonView.Find(targetViewId);
		if (photonView != null && photonView2 != null && photonView.GetComponent<CharacterMotor>() != null)
		{
			string text = GeneralUtils.ColorToHex(GameController.instance.GetTemRelColor(photonView.GetComponent<CharacterMotor>().myTeam));
			string str = "<b><color=#" + text + ">" + photonView.GetComponent<CharacterMotor>().playerInfo.name + "</color></b>";
			string text2 = GeneralUtils.ColorToHex(GameController.instance.GetTemRelColor(photonView2.GetComponent<CharacterMotor>().myTeam));
			string str2 = "<b><color=#" + text2 + ">" + photonView2.GetComponent<CharacterMotor>().playerInfo.name + "</color></b>";
			string message = str + "  killed  " + str2;
			GetComponent<ChatController>().AddSimpleMessage(message, sync: false);
		}
	}

	public void OnDie()
	{
		if (isInCar && base.photonView.isMine)
		{
			CarOrPlayerSwitcher.instance.GetOutFromCar();
		}
		if (base.photonView.isMine)
		{
			playerWeaponManager.CurrentWeapon.ResetAmmoCount();
			WriteChatFragMsg(lastDamagedMeViewId, base.photonView.viewID);
			StopShooting();
			GameInputController.instance.StopAll();
		}
		characterController.enabled = false;
		if (base.photonView.isMine)
		{
			characterAudioController.PlayDeathSound();
		}
		characterAnimation.OnDie();
		if (PlayerCrashed != null)
		{
			PlayerCrashed(this);
		}
		GameController.instance.OnPlayerKilled(this);
		isNowShooting = false;
		HUDManager.instance.ShowHUDofPlayer(this, show: false);
		StartCoroutine(DieCRT());
	}

	private IEnumerator DieCRT()
	{
		if (base.photonView.isMine)
		{
			PhotonView photonView = PhotonView.Find(lastKillerId);
			if (photonView == null)
			{
				CarOrPlayerSwitcher.instance.ActivateKillercamera(base.transform);
			}
			else
			{
				CarOrPlayerSwitcher.instance.ActivateKillercamera(photonView.transform);
			}
		}
		PlayKillFX();
		yield return null;
	}

	private void PlayKillFX()
	{
		if (!(killFXgo != null))
		{
			playerClothingManager.HideAll(hide: true);
			killFXgo = WeaponsPoolManager.instance.PlayPlayerKillFX();
			killFXgo.transform.position = base.transform.position;
			BonusAudioSource.PlayOneShot(killSXs[UnityEngine.Random.Range(0, killSXs.Length)]);
		}
	}

	public void Respawn()
	{
		if (base.photonView.isMine)
		{
			base.gameObject.SetActive(value: true);
			PhotonNetwork.RPC(base.photonView, "RespawnRPC", PhotonTargets.All, false);
			playerWeaponManager.CurrentWeapon.ResetAmmoCount();
		}
		myDamagers.Clear();
	}

	[PunRPC]
	private void RespawnRPC()
	{
		killFXgo = null;
		StartCoroutine(PlayRespawnEffectCRT());
	}

	private IEnumerator PlayRespawnEffectCRT()
	{
		lastDamagedMeViewId = 0;
		characterAnimation.OnRespawn();
		if (base.photonView.isMine)
		{
			Transform transform = ArenaScript.instance.GetRandomRespawnPoint();
			if (GameController.gameConfigData.gameMode == GameMode.TeamFight || GameController.gameConfigData.gameMode == GameMode.CaptureFlag)
			{
				transform = ArenaScript.instance.GetTeamRespawnPoint(myTeam);
			}
			base.transform.position = transform.position;
		}
		playerWeaponManager.OnRespawn();
		if (PlayerRespawned != null)
		{
			PlayerRespawned(this);
		}
		yield return null;
		playerClothingManager.HideAll(hide: false);
		characterController.enabled = true;
		yield return new WaitForSeconds(0.5f);
		if (base.photonView.isMine)
		{
			GetComponent<DamageReciver2>().Heal(playerInfo.max_hp);
		}
		HUDManager.instance.ShowHUDofPlayer(this, show: true);
	}

	private void OnDestroy()
	{
		if (GameController.instance != null)
		{
			GameController.instance.OnOtherPlayerDisconnected(this);
			string str = "<b><color=#" + GeneralUtils.ColorToHex(DataModel.instance.TeamColor(myTeam)) + ">" + playerInfo.name + "</color></b>";
			if (base.photonView.isMine)
			{
			}
			GetComponent<ChatController>().AddSimpleMessage(str + " has left game!", sync: false);
		}
	}

	public void SelectNextWeapon(int sign)
	{
		playerWeaponManager.SelectNextWeapon(sign);
	}

	public void SelectWeapon(int weaponId)
	{
		playerWeaponManager.SelectWeaponById(weaponId);
	}

	private void ResetFragsCount()
	{
		fragsCount = 0;
		playerInfo.deathCount = 0;
		playerInfo.killAssistsCount = 0;
		SetScore(0);
		SynsProps();
	}

	public float GetRunDir()
	{
		if (moveNormalDir.y > 0f)
		{
			return 1f;
		}
		if (moveNormalDir.y < 0f)
		{
			return 0f;
		}
		if (moveNormalDir.x > 0f)
		{
			return 1f;
		}
		if (moveNormalDir.x < 0f)
		{
			return 0f;
		}
		return 0.5f;
	}

	public bool IsDestructed()
	{
		return (playerInfo.hitPoints > 0f) ? true : false;
	}

	public float GetWeaponIndex()
	{
		return playerWeaponManager.CurrentWeaponIndex;
	}

	public void OnCursorFindAim()
	{
		if (!isNowShooting)
		{
			StartShoot();
		}
	}

	public void OnCursorLostAim()
	{
		StopShooting();
	}

	private void HighlightEnemies()
	{
		if (!(GameController.instance == null))
		{
			foreach (CharacterMotor player in GameController.instance.Players)
			{
				if (player == null)
				{
					break;
				}
				if (GameController.gameConfigData.gameMode == GameMode.TeamFight && GameController.instance.IsTeamMate(player.myTeam))
				{
					player.lastAtGunPointTime = Time.time;
				}
				else
				{
					ray.origin = base.transform.position + Vector3.up;
					ray.direction = player.transform.position - base.transform.position;
					if (Physics.Raycast(ray, out hitinfo, 120f) && hitinfo.collider.GetComponent<CharacterMotor>() != null)
					{
						player.lastAtGunPointTime = Time.time;
					}
				}
			}
		}
	}

	public void ResetOnPlayAgain()
	{
		if (base.photonView.isMine)
		{
			GetComponent<DamageReciver2>().Heal(playerInfo.max_hp);
			ResetFragsCount();
			myJetPack.AddFuel(10f);
		}
	}

	public void EnterPauseMode()
	{
		isPaused = true;
		if (!isInCar && base.photonView.isMine)
		{
			base.transform.position = ArenaScript.instance.GetHiddedRespawnPoint().position;
		}
		if (base.photonView.isMine)
		{
			GameInputController.instance.StopAll();
			StopShooting();
		}
		PhotonNetwork.RPC(base.photonView, "EnterPauseModeRPC", PhotonTargets.All, false);
		characterAnimation.EnterPauseMode();
	}

	[PunRPC]
	private void EnterPauseModeRPC()
	{
		isPaused = true;
		if (playerWeaponManager != null && playerWeaponManager.CurrentWeapon != null)
		{
			playerWeaponManager.CurrentWeapon.StopShooting();
		}
	}

	public void OutFromPauseMode()
	{
		isPaused = false;
		if (!isInCar)
		{
			CarOrPlayerSwitcher.instance.OnOutFromPauseMode();
			if (base.photonView.isMine)
			{
				base.transform.position = ArenaScript.instance.GetTeamRespawnPoint(myTeam).position;
			}
		}
		PhotonNetwork.RPC(base.photonView, "OutFromPauseModeRPC", PhotonTargets.All, false);
		if (base.photonView.isMine)
		{
			GameInputController.instance.StopAll();
		}
	}

	[PunRPC]
	private void OutFromPauseModeRPC()
	{
		isPaused = false;
		base.gameObject.SetActive(value: true);
		isInPauseMode = false;
		characterAnimation.OnRespawn();
		playerWeaponManager.OnRespawn();
	}

	public void OnEnterCar(Vehicle car)
	{
		if (base.photonView.isMine)
		{
			if (!car.photonView.isMine)
			{
				car.photonView.RequestOwnership();
			}
			mySittingCarViewId = car.photonView.viewID;
		}
		PhotonNetwork.RPC(base.photonView, "OnEnterCarRPC", PhotonTargets.All, false, car.photonView.viewID);
	}

	[PunRPC]
	private void OnEnterCarRPC(int carViewId)
	{
		isInCar = true;
		PhotonView photonView = PhotonView.Find(carViewId);
		if (photonView != null)
		{
			base.transform.SetParent(photonView.transform);
			Vehicle component = photonView.GetComponent<Vehicle>();
			if (base.photonView.isMine)
			{
				component.OnPlayerSitMe(this);
			}
			else
			{
				component.OnPlayerSitMeRPC(base.photonView.viewID);
			}
			if (component.PlayerPlacePivot != null)
			{
				base.transform.position = component.PlayerPlacePivot.position;
				base.transform.SetParent(component.PlayerPlacePivot);
			}
			else
			{
				base.transform.localPosition = Vector3.zero;
			}
			if (component.vehicleType == VehicleType.Car || component.vehicleType == VehicleType.Heli || component.vehicleType == VehicleType.Plane)
			{
			}
			base.gameObject.GetComponent<CharacterInputController>().Disable();
			characterController.enabled = false;
			component.myDriver = this;
			base.transform.localRotation = Quaternion.identity;
			if (PlayerSitCar != null)
			{
				PlayerSitCar(photonView.GetComponent<Vehicle>());
			}
		}
		EnableShadow(enable: false);
	}

	public void OnLeaveCar(Vehicle car)
	{
		PhotonNetwork.RPC(base.photonView, "OnLeaveCarRPC", PhotonTargets.All, false, car.photonView.viewID);
	}

	[PunRPC]
	private void OnLeaveCarRPC(int carViewId)
	{
		isInCar = false;
		PhotonView photonView = PhotonView.Find(carViewId);
		mySittingCarViewId = -1;
		if (photonView != null)
		{
			if (PlayerLeaveCar != null)
			{
				PlayerLeaveCar(photonView.GetComponent<Vehicle>());
			}
			if (base.photonView.isMine)
			{
				photonView.GetComponent<Vehicle>().OnPlayerLeaveMe(this);
				base.gameObject.GetComponent<CharacterInputController>().Enable();
				base.transform.position = photonView.transform.position + Vector3.up * 2f;
				Transform transform = base.transform;
				Vector3 eulerAngles = photonView.transform.eulerAngles;
				transform.SetEulerY(eulerAngles.y);
			}
			photonView.GetComponent<Vehicle>().myDriver = null;
		}
		base.transform.SetParent(null);
		base.transform.SetEulerX(0f);
		base.transform.SetEulerZ(0f);
		characterController.enabled = true;
	}

	private void OnBecameSceneObject()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	public void AddExp(int exp)
	{
		if (base.photonView.isMine)
		{
			StatisticsManager.AddExp(exp);
		}
	}

	private void CheckOptimization()
	{
		if (Device.isWeakDevice)
		{
			EnableShadow(enable: false);
		}
	}

	public void EnableShadow(bool enable)
	{
		shadowPlane.SetActive(enable);
	}

	private void RaycastShadowPos()
	{
		if (Physics.Linecast(base.transform.position + Vector3.up * 0.1f, base.transform.position + Vector3.down * 10f, out hitinfo, shadowPlaneLayers, QueryTriggerInteraction.Ignore))
		{
			shadowPlane.SetActive(value: true);
			shadowPlane.transform.position = hitinfo.point + hitinfo.normal * 0.02f;
			shadowPlane.transform.forward = hitinfo.normal;
		}
		else
		{
			shadowPlane.SetActive(value: false);
		}
	}

	public void CallBodyguard(string guardId)
	{
		myBodyguard = PhotonNetwork.Instantiate(DataModel.instance.BodyguardPrefabName(guardId), base.transform.position + base.transform.forward * 3f - base.transform.right * 2f, base.transform.rotation, 0).GetComponent<Bodyguard>();
		myBodyguard.SetOwner(this);
	}

	public void DestroyCurrentBodyguard()
	{
		if (myBodyguard != null)
		{
			PhotonNetwork.Destroy(myBodyguard.gameObject);
		}
	}

	public bool IsThisIdKillerAssist(int damagerId)
	{
		return myDamagers.Contains(damagerId);
	}

	public void AddScore(int score)
	{
		playerInfo.score += score;
		GameController.instance.AddTeamScore(score, myTeam);
		if (ScoreChanged != null)
		{
			ScoreChanged(this);
		}
	}

	public void SetScore(int score)
	{
		playerInfo.score = score;
		if (ScoreChanged != null)
		{
			ScoreChanged(this);
		}
	}

	public void HandleMakeKill()
	{
		fragsCount++;
		SynsProps();
		if (base.photonView.isMine)
		{
			characterAudioController.PlayMakeKillSound();
		}
	}

	public void HandleMakeKillAssist()
	{
		playerInfo.killAssistsCount++;
		SynsProps();
	}

	public void HandleMyDeath()
	{
		playerInfo.deathCount++;
		SynsProps();
	}

	public void HandleRevenge()
	{
		lastKillerId = -1;
	}

	private void SynsProps()
	{
		PhotonNetwork.RPC(base.photonView, "SynsPropsR", PhotonTargets.Others, false, fragsCount, playerInfo.killAssistsCount, playerInfo.deathCount, playerInfo.score);
	}

	[PunRPC]
	private void SynsPropsR(int killsCount, int assistsCount, int deathsCount, int score)
	{
		fragsCount = killsCount;
		playerInfo.killAssistsCount = assistsCount;
		playerInfo.deathCount = deathsCount;
		SetScore(score);
		GameWindow.instance.UpdateFragsCount();
	}

	public void ThrowGrenade(int grenadeType)
	{
		if (base.photonView.isMine && (grenadeType != 2 || playerWeaponManager.grenadesCount != 0) && (grenadeType != 4 || playerWeaponManager.smokeGrenadesCount != 0) && (grenadeType != 5 || playerWeaponManager.molotovGrenadesCount != 0))
		{
			StartCoroutine(RotateToTargetPoint(null));
			PhotonNetwork.RPC(base.photonView, "ThrowGrenadeRPC", PhotonTargets.All, false, (byte)grenadeType);
		}
	}

	[PunRPC]
	public void ThrowGrenadeRPC(byte grenadeType)
	{
		bufGrenadeType = grenadeType;
		characterAnimation.ThrowGrenade();
		if (playerWeaponManager.CurrentWeapon != null)
		{
			playerWeaponManager.CurrentWeapon.OnPlyerThrowGrenade();
		}
	}

	public void ThrowGrenadeAnimEvent()
	{
		if (base.photonView.isMine)
		{
			playerWeaponManager.grenadesCount -= ((bufGrenadeType == 2) ? 1 : 0);
			playerWeaponManager.smokeGrenadesCount -= ((bufGrenadeType == 4) ? 1 : 0);
			playerWeaponManager.molotovGrenadesCount -= ((bufGrenadeType == 5) ? 1 : 0);
			GameWindow.instance.SetGrenades(playerWeaponManager.grenadesCount, playerWeaponManager.smokeGrenadesCount, playerWeaponManager.molotovGrenadesCount);
			PhotonNetwork.RPC(base.photonView, "ThrowGrenadeEvR", PhotonTargets.All, false, grenagePivot.position, grenagePivot.forward, bufGrenadeType);
		}
	}

	[PunRPC]
	public void ThrowGrenadeEvR(Vector3 pos, Vector3 dir, byte grenadeType)
	{
		grenade = WeaponsPoolManager.instance.SpawnGrenade(pos, Quaternion.Euler(dir), grenadeType).GetComponent<ArmGrenade>();
		grenade.transform.forward = dir;
		grenade.Throw(13f, 11f);
		grenade.parentView = base.photonView;
	}

	public void ThrowGrenadeAnimEventEnd()
	{
		playerWeaponManager.HideCurrentGun(hide: false);
	}
}
