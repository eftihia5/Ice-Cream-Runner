using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    // Στοιχεία UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endGameText;
    public Button restartButton;
    public GameObject gameLogo;


    // Εφέ και Μεταβλητές παιχνιδιού
    public ParticleSystem dirtParticle;
    public int score = 0;
    public int obstaclesPassed = 0;
    public float jumpForce = 8f;


    // Ήχος
    public AudioSource playerAudio;
    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip crouchSound;
    public AudioClip collectSound;
    public AudioSource bgMusic;

    // Εσωτερικές μεταβλητές
    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded;
    public bool isGameOver = false;

    void Start()
    {
        // Αρχικοποίηση αναφορών
        isGrounded = true;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        UpdateScoreUI();

        playerAudio = GetComponent<AudioSource>();
        
        anim.SetFloat("Speed_f", 1.0f); // ξεκινάει το animation τρεξίματος 

        // Δημιουργία και έναρξη των σωματιδίων σκόνης
        if (dirtParticle != null)
        {
            dirtParticle = Instantiate(dirtParticle, transform.position, Quaternion.identity, transform);
            dirtParticle.Play();
        }
    }

    void Update()
    {
        // Αν το παιχνίδι τελείωσε, μην εκτελείς τίποτα από τα παρακάτω
        if (isGameOver) {
            // Ελέγχουμε αν το restartButton υπάρχει και αν έχει ενεργοποιηθεί στην οθόνη
            if (restartButton != null && restartButton.gameObject.activeInHierarchy)
            {
                // Αν πατήσει Space ή Enter, θα κάνει επανεκκίνηση
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    RestartGame();
                }
            }
        }

        // Έλεγχος νίκης
        if (obstaclesPassed >= 20) 
        {
            WinGame();
        }

        if (!isGameOver)
        {
            // Άλμα
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false; 
                anim.SetTrigger("Jump_trig"); // ενεργοποίηση animation άλματος
                playerAudio.PlayOneShot(jumpSound, 1.0f); // αναπαραγωγή ήχου

                if (dirtParticle != null) dirtParticle.Stop(); // σταματάει η σκόνη
            }

            // Σκύψιμο
            if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
            {
                anim.SetBool("Crouch_b", true); // animation σκυψίματος
                // Μείωση μεγέθους παίκτη για να περνάει κάτω από τα εμπόδια
                transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);

                if (crouchSound != null) {
                    playerAudio.PlayOneShot(crouchSound, 1.0f);
                }

            }

            // Επαναφορά από σκύψιμο όταν αφήσει το πλήκτρο
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                anim.SetBool("Crouch_b", false);
                transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            }
        }
    }

    // Διαχείριση συγκρούσεων
    private void OnCollisionEnter(Collision collision)
    {
        // Αν χτυπήσει σε εμπόδιο
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            playerAudio.PlayOneShot(hitSound, 1.0f);
            GameOver();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Όσο ακουμπάει το έδαφος
        isGrounded = true;
        if (dirtParticle != null && !dirtParticle.isPlaying && !isGameOver)
        dirtParticle.Play(); // Ξεκινάει πάλι η σκόνη
    }

    private void OnCollisionExit(Collision collision)
    {
        // Όταν φεύγει από το έδαφος
        isGrounded = false;
    }

    // Διαχείριση Triggers
    private void OnTriggerEnter(Collider other)
    {
        // Συλλογή παγωτών
        if (other.CompareTag("IceCream"))
        {
            playerAudio.PlayOneShot(collectSound, 4.0f);
            score++;
            UpdateScoreUI();
            Destroy(other.gameObject); // εξαφάνιση του παγωτού
        }

        // Πέρασμα από εικονικό σημείο ελέγχου για να μετράει τα εμπόδια
        if (other.CompareTag("Goal"))
        {
            obstaclesPassed++;
            Destroy(other.gameObject); 
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = " " + score;
    }

    void GameOver()
    {
        isGameOver = true;
        if (bgMusic != null) bgMusic.Stop(); // σταματάει τη μουσική

        // Ρυθμίσεις animation θανάτου
        anim.SetLayerWeight(4, 1f);
        anim.SetBool("Death_b", true);
        anim.SetInteger("DeathType_int", 1);

        if (dirtParticle != null) dirtParticle.Stop();
        playerAudio.PlayOneShot(hitSound, 1.0f);

        endGameText.text = "GAME OVER";
        endGameText.color = Color.red;

        // Ξεκινάει η αναμονή
        StartCoroutine(ShowEndUIAfterAnimation());
    }

    void WinGame()
    {
        if (isGameOver) return;
        bgMusic.Stop();

        isGameOver = true;
        anim.SetFloat("Speed_f", 0); // ο παίκτης σταματάει να τρέχει

        endGameText.text = "CONGRATULATIONS";
        endGameText.color = Color.green;
        if (dirtParticle != null) dirtParticle.Stop(); 

        
        ShowEndUI();
    }

    // Συνάρτηση που καλείται από το Restart Button στο Unity Editor
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Εμφάνιση του UI
    void ShowEndUI()
    {
        endGameText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        if(gameLogo != null) gameLogo.SetActive(true);
    }

    // Περιμένει να τελειώσει το animation θανάτου πριν δείξει το μενού
   private IEnumerator ShowEndUIAfterAnimation()
    {
        // Περίμενε μέχρι να ξεκινήσει το death animation
        yield return new WaitUntil(() => 
            !anim.GetCurrentAnimatorStateInfo(4).IsName("Alive"));

        // Περίμενε μέχρι να μπει σε μια από τις Dead καταστάσεις
        yield return new WaitUntil(() => 
            anim.GetCurrentAnimatorStateInfo(4).IsName("Dead_01") ||
            anim.GetCurrentAnimatorStateInfo(4).IsName("Dead_02"));

        // Περίμενε μέχρι το animation να φτάσει στο τέλος του
        yield return new WaitUntil(() => 
            anim.GetCurrentAnimatorStateInfo(4).normalizedTime >= 1f);

        ShowEndUI();
    }
}