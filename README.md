# Agentes de movimiento autónomo

- **Escena:** Ejercicio
- **Script:** EjercicioAI.cs
- **Prefab Robber:** Ejercicio

## Implementaciones de comportamiento

- Seek (perseguir al target)
- Flee (huir del objetivo)
- Wander (deambular aleatoriamente)

## Lógica de estados

En el método _Update()_ gestiona la lógica de los estados del NPC:

- Si el target está mirando al NPC:
  - Si el target está a menos de una distancia dada: Escapa en dirección contraria
  - En caso contrario: Deambula
- Si el target no está mirando al NPC:
  - Si el NPC está próximo al target: Se detiene
  - En caso contrario: Lo persigue

```csharp
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
```

## Métodos

### Determina si el NPC está a una distancia dada del target

Utiliza las distintas propiedades:

```csharp
[SerializeField] int targetInRange = 5;
[SerializeField] int targetInEscape = 10;
```

Método _IsItNext(int distance)_

```csharp
bool IsItNext(int distance)
{
    return Vector3.Distance(transform.position, target.transform.position) < distance;
}
```

### Comprueba si el target está observando al NPC

```csharp
bool TargetCanSeeMe()
{
    Vector3 toAgent = this.transform.position - target.transform.position;
    float lookingAngle = Vector3.Angle(target.transform.forward, toAgent);
    
    if (lookingAngle < 60)
        return true;
    return false;
}
```

### Envía al NPC a una dirección opuesta al target

```csharp
void Flee(Vector3 location)
{
    Vector3 fleeVector = location - this.transform.position;

    agent.SetDestination(this.transform.position - fleeVector);
}
```

### Hace que el NPC deambule por la zona

```csharp
void Wander()
{
    float wanderRadius = 10;
    float wanderDistance = 10;
    float wanderJitter = 1;

    wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                    0,
                                    Random.Range(-1.0f, 1.0f) * wanderJitter);
    wanderTarget.Normalize();
    
    wanderTarget *= wanderRadius;

    Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
    
    Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

    Seek(targetWorld);
}
```

### Envía al NPC a una posición determinada

```csharp
void Seek(Vector3 location)
{
    agent.SetDestination(location);
}
```