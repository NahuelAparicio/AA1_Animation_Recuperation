# AA1_Animation_Recuperation

# Nahuel Aparicio // nahuel.aparicio@enti.cat
# David Soriano // david.soriano@enti.cat

Move target (donde apuntas) -> WASD
Shoot -> presiona espacio al soltar -> shoot
Valor de efecto -> presiona Z o X
Predecir Trayectoria -> presiona I
Para Restart/Respawn -> presiona P

Exercice 1

1.1 -> script "IK_tentacles" function NotifyShoot()
1.2 -> WASD mueve el target (donde apuntas)
1.3 -> script "ForceSlider" y "EffectSlider"
1.4 -> script "IK_scorpion" Respawn()
1.5 -> Usamos Uniformly Accelerated Rectilinear Motion (Xf = Xo + (Vo*t) + (1/2*a*t^2))
para la velocidad inicial aislamos el parametro Vo quedando -> Vo = (Xf - Xo -(1/2 * a * t ^ 2)) / t
Para calcular la posicion instantania usamos la formula Euler aplicando la fuerza magnus Xn + 1 = Xn + Vn * dt

Exercice 2
2.1 -> EffectSlider script & IK_Scorpion function "TargetPos" que dependiendo del efecto cambia el hit de la pelota.
2.2 -> script "MovingBall" -> Update (si no esta chutando) -> Calcula primero "AngularVelocity()" y despues llama
a "RotAxis()" para calcularlo. En el Update, una vez se realiza el chute (isShoot) llamamos a "RotateBall()" que
nos imprimira el texto por pantalla.
2.3 -> script "MovingBall" Update -> input I. Para actualizar el transform de las flechas lo hacemos en el Update 
(si no estamos chutando), en el if(_enabledArrows).
2.4 -> Para actualizar las flechas de velocidad y fuerzas lo hacemos en "BallArrows()" llamado en el Update()
2.5 -> Para calcular la velocidad de rotacion lo haremos mediante: AngularVelocity = Torque * Lerp(0, fuerdaDeEffectoMaximo, effectoActual);
donde Torque lo conseguimos mediante un Cross de punto de impacto y la velocidad linear -> Torque = Cross(puntoImpacto, linearVelocity)
2.6 -> MagnusForce = Cross(velocidadAngular, velocidadLinearInstantanea); Hemos elegido esta formula para calcular el magnus porque es la más
sencilla, sin necesitar hacer calculos extras para situaciones no contempladas, ejemplo, densidad aire.

Exercice 3

3.1 -> script "IK_Scorpion" en la función "UpdateScorpion" mediante raycast actualiza las posiciones.
3.2 -> Obstacles
3.3 -> Not done
3.4 -> script "IK_Scorpion", en "UpdateScorpion" movemos el mainBody añadiendo el offset con las piernas para evitar
que las patas queden en posiciones raras.
3.5 -> script "IK_scorpion" en "UpdateScorpion()" al final de la funcion.
3.6 -> Actualizamos que el escorpion pueda ir en diferentes direciones mirando hacia ellas calculando la rotacion 
en "UpdateScorpion" y posteriormente en "RotateBody()" aplicamos esta rotacion progresivamente.

Exercice 4 (tail)
4.1 -> El target que recibe la tail se mueve dependiendo de la fuerza de effecto (ejercico 2.1)
4.2 -> Not done
4.3 -> Not done

Exercice 4 (Octopus)
4.1 -> MyOctopusController.dll -> ClampRotation()
4.2 -> MyOctopusController.dll -> Lerp line 181 inside update_ccd
4.3 -> MyOctopusController.dll -> ClampRotation(Q q, Q q)