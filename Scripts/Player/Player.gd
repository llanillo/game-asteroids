extends Area2D

export (int) var speed = 500

var _velocity
var _collision_shape
var _mapLimit
var _player_sprite
var _fire_anim
var _fire_sprite

signal hit_signal

# Llamada cuando el nodo entra en escena por primera vez
func _ready():
	_mapLimit = get_viewport_rect().size
	_player_sprite = $Player_Sprite
	_fire_anim = $Fire_Anim
	_fire_sprite = $Fire_Sprite
	_collision_shape = $Collision	
	pass
	
	
# Llamada cada frame. 'delta' es el tiempo transcurrido desde el último frame
func _process(delta):
	handle_player_input(delta)
	handle_player_rotation()
	handle_player_animation()

# Controla el input del usuario
func handle_player_input(delta):
	_velocity = Vector2()
	
	if Input.is_action_pressed("ui_right"):
		_velocity.x += 1
	if Input.is_action_pressed("ui_left"):
		_velocity.x -= 1
	if Input.is_action_pressed("ui_down"):
		_velocity.y += 1
	if Input.is_action_pressed("ui_up"):
		_velocity.y -= 1		
		
	if _velocity.length() > 0:
		_velocity = _velocity.normalized() * speed

	position += _velocity * delta
	
	position.x = clamp(position.x, 0, _mapLimit.x)
	position.y = clamp(position.y, 0, _mapLimit.y)
	
# Maneja la rotación de la nave
func handle_player_rotation():
	if _velocity.x > 0:
		set_rotation_degrees(90)
	elif _velocity.x < 0:
		set_rotation_degrees(-90)
	elif _velocity.y > 0:
		set_rotation_degrees(-180)
	elif _velocity.y < 0:
		set_rotation_degrees(0)
	
func handle_player_animation():
	if _velocity != Vector2.ZERO:
		_player_sprite.animation = "Moving"
		_fire_anim.play("Fire Blink")
	else:
		_player_sprite.animation ="Still"
		_fire_sprite.visible = false
		_fire_anim.stop()
	
func _on_Player_body_entered(body):
	hide()
	emit_signal("hit_signal")
	_collision_shape.disabled = true

func restartPosition(pos):
	position = pos
	show()
	_collision_shape.disabled = false
	
