extends RigidBody2D

export (int) var min_speed = 150
export (int) var max_speed = 250

var sprite
var collision

# Debe coincidir con los nombres de los sprites
var rock_type = ["Big", "Small"] 

func _ready():
	randomize()
	collision = $Collision
	sprite = $Sprite
	sprite.animation = rock_type[randi() % rock_type.size()]
	
	if sprite.animation == "Big":
		collision.scale.x = 1.6
		collision.scale.y = 1.6
