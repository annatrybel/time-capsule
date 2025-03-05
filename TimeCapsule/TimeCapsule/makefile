.PHONY: run-db run-timecapsule run-all migrate-add migrate-update

# Run only the database container in detached mode.
run-db:
	docker-compose up -d db

# Run all containers (db and timecapsule) in detached mode.
run-containers:
	docker-compose up -d

# Run the TimeCapsule application.
run-app:
	$(MAKE) run-containers
	dotnet run --project ./TimeCapsule.csproj

# Add a new EF Core migration.
# To run it use: make migrate-add NAME=InitialCreate
migrate-add:
ifndef NAME
	$(error Please specify the migration name by setting NAME, e.g., make migrate-add NAME=InitialCreate)
endif
	@echo "Adding migration: $(NAME)"
	# Ensure dotnet-ef is installed (ignore error if already installed)
	dotnet tool install --global dotnet-ef || true
	dotnet ef migrations add $(NAME) --project TimeCapsule/TimeCapsule/TimeCapsule.csproj

# Update the database by applying pending migrations.
migrate-update:
	@echo "Updating database"
	dotnet ef database update --project TimeCapsule/TimeCapsule/TimeCapsule.csproj