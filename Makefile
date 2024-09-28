# Define the docker-compose command
DC = docker-compose

# Default target executed when no arguments are given to make.
run: up

# Target for building and running the docker containers
build:
	$(DC) up --build -d
	
# Target for lifting up the docker containers in detached mode
up:
	$(DC) up -d

# Target to stop all running containers
stop:
	$(DC) stop

# Target to clean (stop and remove) all containers
clean:
	$(DC) down

# Target to clean (stop and remove) all containers, volumes, and networks
clean-volumes:
	$(DC) down -v

# Target to view output from containers
logs:
	$(DC) logs

# Target to follow log output from containers
logs-follow:
	$(DC) logs -f
