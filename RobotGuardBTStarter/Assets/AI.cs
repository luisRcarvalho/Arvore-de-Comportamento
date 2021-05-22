using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player; //variavel que pega o transform do player
    public Transform bulletSpawn; //variavel que pega o tranform da onde saira as balas
    public Slider healthBar;  //variavel que pega o slider da barra de vida
    public GameObject bulletPrefab; //prefab das balas

    NavMeshAgent agent; //variavel que pega o componente agent
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f; //vida do player
    float rotSpeed = 5.0f; //velocidade da rotação

    float visibleRange = 80.0f;//variavel da distancia da  visão
    float shotRange = 40.0f;//variavel de distancia que o tiro vai

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>(); //associando o componente navmeshagent a variavel agent
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth", 5, 0.5f);//invoca o metodo que recupera a vida depois de 5segs e fica recuperando a cada 0.5segs
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position); // faz a barra de vida acompanhar a camera
        healthBar.value = (int)health; //referencia o valor da barra de vida a variavel de vida do player
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0); //define a posição da barra de vida
    }

    void UpdateHealth()
    {
        if (health < 100)//se a vida ficar menor que 100, ele recupera 1 de vida
            health++;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")//se colidir com uma bala, ele perde 10 de vida
        {
            health -= 10;
        }
    }
    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));//variavel de vector "dest" que recebe um X e Z aleatorios
        agent.SetDestination(dest);// realiza a movimentação para o dest gerado
        Task.current.Succeed();//define a task com sucesso
    }
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected) Task.current.debugInfo = string.Format("t={0:0.00}", Time.time); //quando a task é pega, mostra o tempo que esta sendo executada
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed(); //se a distancia percorrida for menor que a distancia pra ele parar,considera a task bem sucedida
        }
    }

    [Task]
    public void TargetPlayer()
    {
        target = player.transform.position; //define o alvo sendo o player(ja pegando o position dele)
        Task.current.Succeed();
    }
    [Task]
    public bool Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);//instancia os tiros
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);//adiciona força aos tiros para que eles vão para frente
        return true;
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position; // define a direção sendo ela o alvo(player) menos a sua posição
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);//realiza uma rotação
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        }

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }
    [Task] public void PickDestination(int x, int z)
    {
        Vector3 dest = new Vector3(x, 0, z);//define o destino, sendo o ponto X e Z definidos pelo script
        agent.SetDestination(dest);//faz a movimentação para o destino
        Task.current.Succeed(); 
    }
    [Task]
    bool SeePlayer()
    {
        Vector3 distance = player.transform.position - this.transform.position; //define a direção sendo ela o alvo(player) menos a sua posição 
        RaycastHit hit;//cria um raycast
        bool seeWall = false; //bool para detectar parede
        Debug.DrawRay(this.transform.position, distance, Color.red);//cria um debug para poder visualizar o raycast
        if(Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true; //se o raycast colidir com um objeto com tag wall, ele declara true na booleana
            }
        }

        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        } 
        if(distance.magnitude < visibleRange && !seeWall)
        {
            return true;
        }
        else return false;
    }

    [Task] 
    bool Turn(float angle)
    { 
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true; 
    }
}
