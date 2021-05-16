using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

	float speed = 20.0F;//variavel de velocidade
    float rotationSpeed = 120.0F;//variavel da velocidade de rotação
    public GameObject bulletPrefab;//prefab das balas
    public Transform bulletSpawn;//variavel que pega o tranform da onde sairá as balas

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed; //cria a variavel que movimenta o personagem na vertical ja multiplicando ela pelo speed
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed; //cria a variavel que movimenta o personagem na horizontal ja multiplicando pelo rotationspeed
        translation *= Time.deltaTime;//multiplica a variavel da verticial por time deltaTime
        rotation *= Time.deltaTime;//multiplica a variavel da horizontal por time deltaTime
        transform.Translate(0, 0, translation);//realiza a movimentação na vertical
        transform.Rotate(0, rotation, 0);//realiza a movimentação na horizontal

        if(Input.GetKeyDown("space"))//quando aperta o espaço
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);//instancia as balas no local que fica o spawern
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);//adciona força na bala para que ela vá pra frente
        }
    }
}
