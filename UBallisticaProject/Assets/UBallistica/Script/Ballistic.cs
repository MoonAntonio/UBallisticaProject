//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// Ballistic.cs (11/10/20016)                                              	    \\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Crea la trajectoria de la ballistica   						\\
// Fecha Mod:       (11/10/20016)                                               \\
// Ultima Mod:  Release inicial      											\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonPincho.Extensiones;
#endregion

namespace MoonPincho
{
    public class Ballistic : MonoBehaviour
    {
        #region Variables Privadas
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Transform barril;
        [SerializeField]
        private GameObject bala;
        [SerializeField]
        private bool debugFuego;
        [SerializeField]
        private LineRenderer trail;
        [SerializeField]
        private Color trailColor;
        [SerializeField]
        private int maxSegmentCount = 300;

        private Vector3 v0 = Vector3.zero;
        private Vector3? lastVelocity = Vector3.zero;
        private Vector3? lastTargetPos = null;
        private Collider _hitObject;
        private List<Vector3> segments = new List<Vector3>();
        private RaycastHit hit;
        private Coroutine trailRoutine;
        private Coroutine targetRoutine;
        #endregion

        #region Metodos Comunes
        /// <summary>
        /// <para>Inicializacion</para>
        /// </summary>
        void Awake()
        {
            if (trail == null) trail = GetComponent<LineRenderer>();
            segments.Add(barril.position);
        }

        /// <summary>
        /// <para>Actualizador</para>
        /// </summary>
        void Update()
        {
            if (Input.GetButtonDown("Fire1") || debugFuego)
            {
                GameObject b = Instantiate(bala, barril.position, barril.rotation) as GameObject;
                b.GetComponent<Rigidbody>().velocity = v0;
                Destroy(b, 10);
                debugFuego = false;
            }

            if (lastVelocity != v0)
            {
                targetRoutine = StartCoroutine(target.CrearLineaCirculoCoRoutine(trailColor, 5, 30));
                trailRoutine = StartCoroutine(SimularPath());
            }
            else
            {
                StopAllCoroutines();
                _hitObject = null;
            }

            lastVelocity = v0;
            lastTargetPos = target.position;
        }

        /// <summary>
        /// <para>Actualizador Physico</para>
        /// </summary>
        void FixedUpdate()
        {
            Vector3 delta = target.position - transform.position;
            delta.y = 0;
            Quaternion rot = Quaternion.LookRotation(delta, transform.up);
            Vector3 angles = rot.eulerAngles;
            angles.x = transform.eulerAngles.x;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(angles), Time.fixedDeltaTime * 2);

            Vector3 relative = barril.position - transform.position;
            float angle = Mathf.Atan2(relative.y, relative.z) * Mathf.Rad2Deg;
            v0 = VelocidadBallistica(target.position, angle);
        }
        #endregion

        #region Metodos Simulador
        /// <summary>
        /// <para>Velocidad de la Ballistica</para>
        /// </summary>
        /// <param name="targetPos">Objetivo</param>
        /// <param name="angulo">Angulo</param>
        /// <returns>La velocidad</returns>
        Vector3 VelocidadBallistica(Vector3 targetPos, float angulo)
        {
            Vector3 dir = targetPos - transform.position;                                   // obtener la dirección de destino
            float h = dir.y;                                                                // obtener la diferencia de altura
            dir.y = 0;                                                                      // conservar únicamente la dirección horizontal
            float dist = dir.magnitude;                                                     // obtener la distancia horizontal
            float a = angulo * Mathf.Deg2Rad;                                                // convertir el ángulo en radianes
            dir.y = dist * Mathf.Tan(a);                                                     // fijar el ángulo de elevación
            dist += h / Mathf.Tan(a);                                                       // correcion para las pequeñas diferencias de altura
                                                                                            // el cálculo de la magnitud de la velocidad
            float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return vel * dir.normalized;
        }

        /// <summary>
        /// <para>Simular la ruta</para>
        /// </summary>
        /// <returns></returns>
        IEnumerator SimularPath()
        {
            // Velocidad Inicial
            Vector3 segVelocity = v0;
            Color startColor = trailColor;
            Color endColor = startColor;
            startColor.a = 1;
            endColor.a = 0;

            trail.SetColors(startColor, endColor);

            for (int i = 1; i < maxSegmentCount; i++)
            {
                if (i == 1)
                {
                    segments = new List<Vector3>();
                    segments.Add(barril.position);
                    trail.SetVertexCount(1);
                    trail.SetPosition(0, segments[0]);
                }
                // El tiempo que se tarda en recorrer un segmento de la escala de longitud seg (cuidado si la velocidad es cero)
                float segTime = (segVelocity.sqrMagnitude != 0) ? 1 / segVelocity.magnitude : 0;

                // Añadir la velocidad de la gravedad
                segVelocity = segVelocity + Physics.gravity * segTime;

                // Comprobar para ver si vamos a golpear un objeto physico
                if (Physics.Raycast(segments[i - 1], segVelocity, out hit, 1))
                {
                    // Recordar que golpeamos
                    _hitObject = hit.collider;

                    segments.Add(segments[i - 1] + segVelocity.normalized * hit.distance);
                    // Corregir velocidad
                    //segVelocity = segVelocity - Physics.gravity * (1 - hit.distance) / segVelocity.magnitude;
                    // Voltear la velocidad para simular un rebote
                    //segVelocity = Vector3.Reflect(segVelocity, hit.normal);
                    break;
                }
                // Si nuestro raycasthit no choca con ningún objeto, a continuación, establezca la siguiente posición a la última, v*t
                else
                {
                    segments.Add(segments[i - 1] + segVelocity * segTime);
                }

                trail.SetVertexCount(segments.Count);
                trail.SetPosition(i, segments[i]);
            }
            yield return new WaitForSeconds(.5f);
        }
        #endregion
    }
}