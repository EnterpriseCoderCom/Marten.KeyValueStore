using EnterpriseCoder.Marten.KeyValueStore.UnitTests.TestObjects;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseCoder.Marten.KeyValueStore.UnitTests;

public class KeyValueStoreTests : IClassFixture<DatabaseTestFixture>, IDisposable
{
    private const float Tolerance = 0.00001f;
    
    private readonly DatabaseTestFixture _databaseTestFixture;
    private readonly IKeyValueStore _keyValueStore;
    
    public KeyValueStoreTests(DatabaseTestFixture fixture)
    {
        _databaseTestFixture = fixture;
        _keyValueStore = fixture.ServiceProvider.GetRequiredService<IKeyValueStore>();
    }

    [Fact]
    public async Task ReadWriteDeleteTest()
    {
        await using (var scope = _databaseTestFixture.ServiceProvider.CreateAsyncScope())
        {
            using (var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>())
            {
                // Clean up...
                DatabaseHelper databaseHelper = new DatabaseHelper(documentSession);
                await databaseHelper.ClearDatabaseAsync();
                
                // There shouldn't be a pre-existing value 
                bool value1Present = await _keyValueStore.HasKeyAsync(documentSession, "UnitTesting.Value1");
                Assert.False(value1Present);
                
                // Set Value1
                await _keyValueStore.SetStringAsync(documentSession, "UnitTesting.Value1", "Hello World");
                await documentSession.SaveChangesAsync();
                
                // Verify that that key is now in the database.
                value1Present = await _keyValueStore.HasKeyAsync(documentSession, "UnitTesting.Value1");
                Assert.True(value1Present);
                
                // Read the value of the key to ensure it was stored properly.
                string? value1 = await _keyValueStore.GetStringAsync(documentSession, "UnitTesting.Value1", null);
                Assert.Equal("Hello World", value1);

                // Test the default value on missing key functionality
                string? value2 = await _keyValueStore.GetStringAsync(documentSession, "UnitTesting.Value2", "Test");
                Assert.Equal("Test", value2);
                
                // Delete Value1
                await _keyValueStore.DeleteKeyAsync(documentSession, "UnitTesting.Value1");
                await documentSession.SaveChangesAsync();
                value1Present = await _keyValueStore.HasKeyAsync(documentSession, "UnitTesting.Value1");
                Assert.False(value1Present);
                
                // Delete Value2 
                await _keyValueStore.DeleteKeyAsync(documentSession, "UnitTesting.Value2");
                await documentSession.SaveChangesAsync();
                bool value2Present = await _keyValueStore.HasKeyAsync(documentSession, "UnitTesting.Value2");
                Assert.False(value2Present);
            }
        }
    }

    [Fact]
    public async Task GetSetNonStringsTest()
    {
        await using (var scope = _databaseTestFixture.ServiceProvider.CreateAsyncScope())
        {
            using (var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>())
            {
                // Clean up...
                DatabaseHelper databaseHelper = new DatabaseHelper(documentSession);
                await databaseHelper.ClearDatabaseAsync();

                await _keyValueStore.SetBoolAsync(documentSession, "UnitTesting.Boolean", true);
                await documentSession.SaveChangesAsync();
                bool boolValue = await _keyValueStore.GetBoolAsync(documentSession, "UnitTesting.Boolean", false);
                Assert.True(boolValue);
                
                await _keyValueStore.SetIntAsync(documentSession, "UnitTesting.Integer", 42);
                await documentSession.SaveChangesAsync();
                int intValue = await _keyValueStore.GetIntAsync(documentSession, "UnitTesting.Integer", 0);
                Assert.Equal(42, intValue);
                
                await _keyValueStore.SetLongAsync(documentSession, "UnitTesting.Long", 42L);
                await documentSession.SaveChangesAsync();
                long longValue = await _keyValueStore.GetLongAsync(documentSession, "UnitTesting.Long", 0L);
                Assert.Equal(42L, longValue);
                
                await _keyValueStore.SetFloatAsync(documentSession, "UnitTesting.Float", 42.558f);
                await documentSession.SaveChangesAsync();
                float floatValue = await _keyValueStore.GetFloatAsync(documentSession, "UnitTesting.Float", 0.0f);
                Assert.True(Math.Abs(floatValue - 42.558f) < Tolerance);
                
                await _keyValueStore.SetDoubleAsync(documentSession, "UnitTesting.Double", 42.558);
                await documentSession.SaveChangesAsync();
                double doubleValue = await _keyValueStore.GetDoubleAsync(documentSession, "UnitTesting.Double", 0.0);
                Assert.True(Math.Abs(doubleValue - 42.558) < Tolerance);
                
            }
        }
    }

    [Fact]
    public async Task GetSetObjectTests()
    {
        await using (var scope = _databaseTestFixture.ServiceProvider.CreateAsyncScope())
        {
            using (var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>())
            {
                // Clean up...
                DatabaseHelper databaseHelper = new DatabaseHelper(documentSession);
                await databaseHelper.ClearDatabaseAsync();

                // Create a dog
                Dog dog = new Dog()
                {
                    Name = "Roofus",
                    DogSpecificVariable = "All Too Friendly"
                };
                
                // Create a cat
                Cat cat = new Cat()
                {
                    Name = "Ms. Whiskers",
                    CatSpecificVariable = "Rules with an iron fist"
                };

                // Write the dog.
                await _keyValueStore.SetObjectAsync(documentSession, "UnitTesting.Animal", dog);
                await documentSession.SaveChangesAsync();
                
                // Read the dog as an animal
                Animal? reloadDog =
                    await _keyValueStore.GetObjectAsync<Animal>(documentSession, "UnitTesting.Animal", null);
                Assert.NotNull(reloadDog);
                Assert.Equal(typeof(Dog), reloadDog!.GetType());
                
                // Write the cat as an animal
                Animal catAsAnimal = cat;
                await _keyValueStore.SetObjectAsync(documentSession, "UnitTesting.Animal", catAsAnimal);
                await documentSession.SaveChangesAsync();

                // Read the cat as an animal
                Animal? reloadCat = await _keyValueStore.GetObjectAsync<Animal>(documentSession, "UnitTesting.Animal", null);
                Assert.NotNull(reloadCat);
                Assert.Equal(typeof(Cat), reloadCat!.GetType());

            }
        }
    }

    public void Dispose()
    {
        
    }

}