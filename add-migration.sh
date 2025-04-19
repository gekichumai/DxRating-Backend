if [ -z "$1" ]; then
    echo "Error: MigrationName argument is required"
    echo "Usage: $0 <MigrationName>"
    exit 1
fi

MigrationName="$1"
dotnet ef migrations add --project ./src/DxRating.Database/DxRating.Database.csproj --startup-project ./src/DxRating.Worker.Migrator/DxRating.Worker.Migrator.csproj "$MigrationName"
