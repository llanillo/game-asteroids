extends Area2D

export (int) var speed := 500
export (int) var rotation_speed := 4.5

var velocity : Vector2
var collision_shape : CollisionShape2D
var map_limit : Vector2
var player_anim_sprite : AnimatedSprite
var burst_anim_player : AnimationPlayer
var burst_sprite : Sprite

signal hit_signal
var rotation_direction : int

func _ready():
	map_limit = get_viewport_rect().size
	player_anim_sprite = $Player_AnimSprite
	burst_anim_player = $Burst_AnimPlayer
	burst_sprite = $Burst_Sprite
	collision_shape = $Collision	
	
func _physics_process(delta):
	handle_player_movement(delta)
	
func _process(delta):
	handle_player_input(delta)
	handle_player_animation()

func handle_player_input(delta):
	rotation_direction = 0
	velocity = Vector2()
	
	if Input.is_action_pressed("ui_right"):
		rotation_direction += 1
	elif Input.is_action_pressed("ui_left"):
		rotation_direction -= 1
	elif Input.is_action_pressed("ui_down"):
		velocity = Vector2(speed, 0).rotated(rotation)
	elif Input.is_action_pressed("ui_up"):
		velocity = Vector2(-speed, 0).rotated(rotation)
		
	velocity = velocity.normalized() * speed
	
func handle_player_movement(delta):
	rotation += rotation_direction * rotation_speed * delta
	position += velocity * delta
	
	position.x = clamp(position.x, 0, map_limit.x)
	position.y = clamp(position.y, 0, map_limit.y)
	
func handle_player_animation():
	if velocity != Vector2.ZERO:
		player_anim_sprite.animation = "Moving"
		burst_anim_player.play("Fire Blink")
	else:
		player_anim_sprite.animation ="Still"
		burst_sprite.visible = false
		burst_anim_player.stop()
	
func _on_Player_body_entered(body):
	hide()
	emit_signal("hit_signal")
	collision_shape.set_deferred("disabled", true)
#	collision_shape.disabled = true

func restartPosition(pos):
	position = pos
	show()
	collision_shape.disabled = false
	
