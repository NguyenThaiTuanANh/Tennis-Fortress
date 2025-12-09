using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Speed & Y-lock")]
    public float fixedSpeed = 10f;
    public float fixedY = 1f;

    [Header("Drift toward down")]
    public Vector3 downDirection = new Vector3(0, 0, -1);
    [Range(0f, 1f)]
    public float driftStrength = 1f;

    [Header("Tags")]
    public string playerTag = "Player";
    public string enemyTag = "Enemy";

    [Header("Hit Settings")]
    public float launchSpeedOnHit = 25f;
    public float minDistanceToEnemy = 0.5f;

    [Header("Effects")]
    public GameObject bonusHitEffect;
    public float effectDestroyTime = 1.5f;

    [HideInInspector] public int dameBall = 40;

    private Rigidbody rb;

    [Header("Power & Skill")]
    public GameObject miniBallPrefab;
    public GameObject powerHitEffect;
    public GameObject powerBallEffect;
    private bool isShotBoosted = false;

    [Header("Snake Camera")]
    private Camera mainCam;
    public float snakeDuration = 0.5f;
    public float snakeShakeDuration = 0.15f;
    public float snakeShakeAmplitude = 0.15f;

    private float snakeTimer = 0f;
    private Vector3 camOriginalPos;

    [Header("Ball Mesh Change")]
    public MeshFilter meshFilter;
    public Mesh[] meshLevels;
    public TrailRenderer trail;
    public Material[] trailMat;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        GameObject playerPlain = GameObject.FindGameObjectWithTag("PlayerPlain");
        if (playerPlain)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), playerPlain.GetComponent<Collider>());
        }
    }

    void Start()
    {
        if (mainCam == null)
            mainCam = Camera.main;
        if (mainCam != null)
            camOriginalPos = mainCam.transform.localPosition;
        rb.linearVelocity = rb.linearVelocity.magnitude < 0.1f
            ? downDirection.normalized * fixedSpeed
            : rb.linearVelocity.normalized * fixedSpeed;

        dameBall = PlayerPrefs.GetInt("BallDamage", 40);
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }
    void LateUpdate()
    {
        if (snakeTimer > 0 && mainCam != null)
        {
            snakeTimer -= Time.deltaTime;

            float t = snakeTimer / snakeShakeDuration; 
            float strength = snakeShakeAmplitude * t;  

            float offsetX = (Mathf.PerlinNoise(Time.time * 20f, 0f) - 0.5f) * strength;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * 20f) - 0.5f) * strength;

            mainCam.transform.localPosition = camOriginalPos + new Vector3(offsetX, offsetY, 0);

            if (snakeTimer <= 0)
            {
                mainCam.transform.localPosition = camOriginalPos;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            HandleWallBounce(collision);
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            HandlePlayerHit();
            return;
        }    

        if (other.CompareTag(enemyTag))
        {
            HandleEnemyHit(other);
            return;
        }
    }


    void HandlePlayerHit()
    {
        AudioManager.Instance.PlayPlayerHit();
        snakeTimer = snakeDuration;

        // Kiểm tra multi-shot skill
        if (SkillManager.Instance.multiShotActive)
        {

            isShotBoosted = true;
            ChangeBallMesh(1);
            SetTrailColor(1);
            RedirectToNearestEnemy(2);
            return;
        }


        // Power activated 
        if (PowerBarManager.Instance.isActivated && PowerBarManager.Instance.currentBounce == 0)
        {
            ChangeBallMesh(2);
            SetTrailColor(2);
            PlayerController.Instance.powerPlayerEffect.SetActive(false);
            RedirectToNearestEnemy(2);
            return;
        }

        RedirectToNearestEnemy(1.2f);
    }

    void HandleEnemyHit(Collider collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        
        if (!enemy) return;

        int finalDamage = dameBall;

        if (isShotBoosted)
        {
            Vector3 temp = new Vector3(gameObject.transform.position.x, 1.1f, gameObject.transform.position.z);
            GameObject fxx = Instantiate(bonusHitEffect, temp, Quaternion.identity);
            Destroy(fxx, effectDestroyTime);
            ChangeBallMesh(0);
            SetTrailColor(0);
            finalDamage = Mathf.RoundToInt(dameBall * 1.5f);
            isShotBoosted = false; 
            if (powerBallEffect) powerBallEffect.SetActive(false); 
        }

        enemy.TakeDamage(finalDamage);

        if (PowerBarManager.Instance.isActivated)
        {
            Vector3 tempx = new Vector3(gameObject.transform.position.x, 1.1f, gameObject.transform.position.z);
            GameObject fx = Instantiate(powerHitEffect, tempx, Quaternion.identity);
            Destroy(fx, effectDestroyTime);
            PowerBarManager.Instance.currentBounce++;
            if (PowerBarManager.Instance.currentBounce >= PowerBarManager.Instance.maxPowerBounce)
            {
                ChangeBallMesh(0);
                SetTrailColor(0);
                PowerBarManager.Instance.Consume();
            }
            else 
            {
                Vector3 dirbonus = (FindNearestEnemyTransform().position - transform.position).normalized;
                rb.linearVelocity += dirbonus.normalized * 5f;
                dirbonus.y = 0;
                rb.linearVelocity = dirbonus * launchSpeedOnHit * 1.2f;
                return;
            }
        }

        // Normal random bounce
        Vector3 dir = downDirection.normalized;
        dir = Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.up) * dir;
        dir.y = 0;

        rb.linearVelocity = dir.normalized * fixedSpeed;
    }

    void HandleWallBounce(Collision collision)
    {
        if (PowerBarManager.Instance.isActivated)
            return;
        ContactPoint contact = collision.GetContact(0);
        Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, contact.normal);
        reflectDir.y = 0;

        Vector3 nudged = Vector3.Slerp(reflectDir, downDirection.normalized, 0.02f).normalized;


        rb.linearVelocity = nudged * fixedSpeed;
        ApplySmartDrift();
    }

    void ApplySmartDrift()
    {
        if (PowerBarManager.Instance.isActivated)
            return;
        Vector3 v = rb.linearVelocity;
        v.y = 0;

        Vector3 desired = downDirection.normalized;

        float angle = Vector3.Angle(v, desired);

        if (angle > 45f)
        {
            Vector3 corrected = Vector3.Lerp(v, desired, driftStrength);
            rb.linearVelocity = corrected.normalized * rb.linearVelocity.magnitude;
        }
    }


    void RedirectToNearestEnemy( float speed)
    {
        Transform nearest = FindNearestEnemyTransform();
        if (!nearest) Debug.Log("AnhNTT Find Enemy null ");
        if (!nearest) return;

        Vector3 dir = (nearest.position - transform.position);
        //if (dir.sqrMagnitude < minDistanceToEnemy * minDistanceToEnemy)
        //    dir += dir.normalized * 0.5f;

        dir.y = 0;
        rb.linearVelocity = dir.normalized * launchSpeedOnHit * speed;
        Debug.Log("Speed to enemy : " + rb.linearVelocity);
    }

    Transform FindNearestEnemyTransform()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return null;

        Debug.Log("AnhNTT enemy gan nhat: " + enemies
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .First().transform.position);
        return enemies
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .First()
            .transform;
    }

    public void ChangeBallMesh(int index)
    {
        if (meshFilter == null || meshLevels.Length == 0) return;
        if (index < 0 || index >= meshLevels.Length) return;

        meshFilter.mesh = meshLevels[index];
    }

    public void SetTrailColor(int index)
    {

        if (trail != null)
        {
            trail.material = trailMat[index];
        }
    }

}
