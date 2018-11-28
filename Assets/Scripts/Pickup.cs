using UnityEngine;

public enum pickupType { Dot, Energizer, Fruit }

public class Pickup : MonoBehaviour {

	public int point;
    public pickupType type;
    private AudioSource aud;
    private GameManager gm;

    void Start() {
        gm = GameObject.Find("Managers").GetComponent<GameManager>();
		aud = GetComponent<AudioSource>();
    }

	void OnTriggerEnter(Collider other) {
        if (other.name == "player") {
            
            aud.Play();
            gm.addScore(point);
            if (type == pickupType.Energizer) {
                // powerup the pacman and scare the ghosts 
                gm.CounterAttack();
            }
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 2.0f);
            //Destroy(gameObject, aud.clip.length);
		}
	}
}
