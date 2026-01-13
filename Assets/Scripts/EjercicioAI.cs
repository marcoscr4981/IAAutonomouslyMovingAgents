using UnityEngine;
using UnityEngine.AI;

//=============================================================================
// Controla o comportamento dun axente autónomo que se move polo NavMesh
// Implementa o comportamento de:
//      - Seek (perseguir al target)
//      - Flee (huir del objetivo)
//      - Wander (deambular aleatoriamente)
//=============================================================================
public class EjercicioAI : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] int targetInRange = 5;
    [SerializeField] int targetInEscape = 10;

    NavMeshAgent agent;
    Vector3 wanderTarget = Vector3.zero;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        // Si el target está viendo al NPC
        if (TargetCanSeeMe())
        {
            // Si el target está a una distancia dada, el NPC escapa
            // En caso contrario deambula
            if (IsItNext(targetInEscape))
            {
                Flee(target.transform.position);
            }
            else
            {
                Wander();
            }
        }
        else
        {
            // Si el NPC está cerca del target, el NPC se detiene
            // En caso contrario lo persigue
            if (IsItNext(targetInRange))
            {
                agent.ResetPath();
            }
            else
            {
                Seek(target.transform.position);
            }
        }
    }


    // Determina si el NPC está a una distancia dada del target
    bool IsItNext(int distance)
    {
        return Vector3.Distance(transform.position, target.transform.position) < distance;
    }


    // Si el objetivo puede ver al NPC
    bool TargetCanSeeMe()
    {
        // Calcular unha dirección frontal para o obxectivo e
        // O ángulo entre iso e a dirección ao axente
        Vector3 toAgent = this.transform.position - target.transform.position;
        float lookingAngle = Vector3.Angle(target.transform.forward, toAgent);

        // Se o obxectivo está mirando ao axente dentro dun rango de 60 graos
        // Asumimos que o obxectivo pode ver o axente
        if (lookingAngle < 60)
            return true;
        return false;
    }


    // Envía al NPC a la dirección opuesta del target
    void Flee(Vector3 location)
    {
        //calcular o vector na dirección contraria á localización
        //este é 180 graos ao vector cara á localización
        Vector3 fleeVector = location - this.transform.position;

        //restar este vector da posición do axente e 
        //establecer isto como a nova localización no NavMesh
        agent.SetDestination(this.transform.position - fleeVector);
    }


    // El NPC deambula por el mapa de forma aleatoria usando un algoritmo de círculo de deambulación
    void Wander()
    {
        float wanderRadius = 10; // Radio do círculo de deambulación
        float wanderDistance = 10; // Distancia do círculo á fronte do axente
        float wanderJitter = 1; // Cantidade de aleatoriedade aplicada cada frame

        //determinar unha localización nun círculo 
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                        0,
                                        Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        //proxectar o punto ao radio do círculo
        wanderTarget *= wanderRadius;

        //mover o círculo cara adiante do axente á distancia de deambulación
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        //calcular a localización mundial do punto no círculo
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        Seek(targetWorld);
    }


    // Envía al agente a una localización del NavMesh
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }
}
