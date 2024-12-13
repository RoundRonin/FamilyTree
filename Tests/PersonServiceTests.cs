using FamilyTreeBlazor.BLL;
using FamilyTreeBlazor.BLL.DTOs;
using FamilyTreeBlazor.DAL.Entities;
using FamilyTreeBlazor.DAL.Infrastructure;
using Moq;
using Xunit;

namespace FamilyTreeBlazor.Tests
{
    public class PersonServiceTests
    {
        private readonly Mock<IRepository<Person>> _personRepositoryMock;
        private readonly TreeCacheDTO _treeCache;
        private readonly PersonService _personService;

        public PersonServiceTests()
        {
            _personRepositoryMock = new Mock<IRepository<Person>>();
            _treeCache = new TreeCacheDTO();
            _personService = new PersonService(_personRepositoryMock.Object, _treeCache);
        }

        // AddPersonAsync Tests
        [Fact]
        public async Task AddPersonAsync_ShouldAddPerson()
        {
            // Arrange
            var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);

            // Act
            await _personService.AddPersonAsync(person);

            // Assert
            _personRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Person>()), Times.Once);
            Assert.True(_treeCache.Persons.ContainsKey(person.Id));
        }

        [Fact]
        public async Task AddPersonAsync_WithExistingRelationships_ShouldAddPersonWithRelationships()
        {
            // Arrange
            var parent = new PersonDTO(2, "Jane Doe", new DateTime(1955, 1, 1), false);
            _treeCache.Persons[parent.Id] = parent;

            var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true)
            {
                Parents = new List<PersonDTO> { parent }
            };

            // Act
            await _personService.AddPersonAsync(person);

            // Assert
            _personRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Person>()), Times.Once);
            Assert.True(_treeCache.Persons.ContainsKey(person.Id));
            Assert.Contains(parent, person.Parents);
        }

        // GetPersonById Tests
        [Fact]
        public void GetPersonById_ShouldReturnPerson()
        {
            // Arrange
            var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
            _treeCache.Persons[person.Id] = person;

            // Act
            var result = _personService.GetPersonById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(person.Id, result.Id);
        }

        [Fact]
        public void GetPersonById_InvalidId_ShouldReturnNull()
        {
            // Act
            var result = _personService.GetPersonById(99);

            // Assert
            Assert.Null(result);
        }

        // GetAllPersonsAsync Tests
        [Fact]
        public async Task GetAllPersonsAsync_ShouldReturnAllPersons()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person(1, "John Doe", new DateTime(1980, 1, 1), true),
                new Person(2, "Jane Doe", new DateTime(1955, 1, 1), false)
            };
            _personRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(persons);

            // Act
            var result = await _personService.GetAllPersonsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPersonsAsync_EmptyDatabase_ShouldReturnEmpty()
        {
            // Arrange
            var persons = new List<Person>();
            _personRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(persons);

            // Act
            var result = await _personService.GetAllPersonsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdatePersonAsync_ShouldUpdatePerson()
        {
            // Arrange
            var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
            var personEntity = new Person(person.Id, person.Name, person.BirthDateTime, person.Sex);
            
            _treeCache.Persons[person.Id] = person;
            _personRepositoryMock.Setup(repo => repo.RetrieveByIdAsync(person.Id)).ReturnsAsync(personEntity);

            var updatedPerson = new PersonDTO(1, "John Updated", new DateTime(1980, 1, 1), true);

            // Act
            await _personService.UpdatePersonAsync(updatedPerson);

            // Assert
            _personRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Person>()), Times.Once);
            Assert.Equal("John Updated", _treeCache.Persons[person.Id].Name);
        }

        [Fact]
        public async Task UpdatePersonAsync_NonExistentPerson_ShouldThrowException()
        {
            // Arrange
            var updatedPerson = new PersonDTO(99, "Non Existent", new DateTime(1980, 1, 1), true);
            _personRepositoryMock.Setup(repo => repo.RetrieveByIdAsync(updatedPerson.Id)).ReturnsAsync((Person)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _personService.UpdatePersonAsync(updatedPerson));

            Assert.Equal($"Person with ID {updatedPerson.Id} does not exist.", exception.Message);
            _personRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Person>()), Times.Never);
        }


        // DeletePersonAsync Tests
        [Fact]
        public async Task DeletePersonAsync_ShouldDeletePerson()
        {
            // Arrange
            var person = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
            _treeCache.Persons[person.Id] = person;

            // Act
            await _personService.DeletePersonAsync(person.Id);

            // Assert
            _personRepositoryMock.Verify(repo => repo.DeleteAsync(person.Id), Times.Once);
            Assert.False(_treeCache.Persons.ContainsKey(person.Id));
        }

        [Fact]
        public async Task DeletePersonAsync_NonExistentPerson_ShouldNotThrow()
        {
            // Act
            var exception = await Record.ExceptionAsync(() => _personService.DeletePersonAsync(99));

            // Assert
            Assert.Null(exception);
            _personRepositoryMock.Verify(repo => repo.DeleteAsync(99), Times.Once);
        }

        // ClearAllAsync Tests
        [Fact]
        public async Task ClearAllAsync_ShouldClearAllPersons()
        {
            // Act
            await _personService.ClearAllDbAsync();

            // Assert
            _personRepositoryMock.Verify(repo => repo.TruncateTableAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearAllAsync_AfterMultipleOperations_ShouldClearAllPersons()
        {
            // Arrange
            var person1 = new PersonDTO(1, "John Doe", new DateTime(1980, 1, 1), true);
            var person2 = new PersonDTO(2, "Jane Doe", new DateTime(1955, 1, 1), false);
            await _personService.AddPersonAsync(person1);
            await _personService.AddPersonAsync(person2);

            // Act
            await _personService.ClearAllDbAsync();

            _personRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Person>());

            var result = await _personService.GetAllPersonsAsync();

            // Assert
            _personRepositoryMock.Verify(repo => repo.TruncateTableAsync(), Times.Once);
            Assert.Empty(result);
        }
    }
}
