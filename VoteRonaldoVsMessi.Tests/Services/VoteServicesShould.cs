using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoteRonaldoVsMessi.Infrastructure;
using VoteRonaldoVsMessi.Infrastructure.Models;
using VoteRonaldoVsMessi.Services;
using Xunit;

namespace VoteRonaldoVsMessi.Tests.Services
{
    public class VoteServicesShould
    {
        [Fact]
        public async Task GetVoteByGUID()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();
            var fakeVotedFor = "Messi";
            var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

            using (var context = GetContext(dbName))
            {
                context.Votes.Add(fakeVote);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var ActualVote = await service.GetVoteAsync(fakeVoteId);

                //Assert
                Assert.Equal(fakeVote.VoterId, ActualVote.VoterId);
                Assert.Equal(fakeVote.VotedFor, ActualVote.VotedFor);
            }
        }

        [Fact]
        public async Task GetVoteByStringId()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();
            var fakeVotedFor = "Messi";
            var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

            using (var context = GetContext(dbName))
            {
                context.Votes.Add(fakeVote);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var ActualVote = await service.GetVoteAsync(fakeVoteId.ToString());

                //Assert
                Assert.Equal(fakeVote.VoterId, ActualVote.VoterId);
                Assert.Equal(fakeVote.VotedFor, ActualVote.VotedFor);
            }
        }

        [Fact]
        public async Task GetAllVotes()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var numberOfVotes = 5;
            var fakeVotes = new List<Vote>();

            using (var context = GetContext(dbName))
            {
                fakeVotes = GetVotes(numberOfVotes);
                for (int i = 0; i < numberOfVotes; i++)
                {
                    context.Votes.AddRange(fakeVotes);
                }
                await context.SaveChangesAsync();
            }

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var ActualVotes = await service.GetVotesAsync();

                for (int i = 0; i < numberOfVotes; i++)
                {
                    //Assert
                    Assert.Equal(fakeVotes[i].VoterId, ActualVotes[i].VoterId);
                    Assert.Equal(fakeVotes[i].VotedFor, ActualVotes[i].VotedFor);
                }
            }
        }

        [Fact]
        public async Task TellIfVoteExistsByGUID()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();
            var fakeVotedFor = "Messi";
            var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

            using (var context = GetContext(dbName))
            {
                context.Votes.Add(fakeVote);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var isVoteFound = await service.IsVoteExistsAsync(fakeVoteId);

                //Assert
                Assert.True(isVoteFound);
            }
        }

        [Fact]
        public async Task TellIfVoteExistsByStringId()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();
            var fakeVotedFor = "Messi";
            var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

            using (var context = GetContext(dbName))
            {
                context.Votes.Add(fakeVote);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var isVoteFound = await service.IsVoteExistsAsync(fakeVoteId.ToString());

                //Assert
                Assert.True(isVoteFound);
            }
        }

        [Fact]
        public async Task TellIfVoteDoesNotExistByGUID()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var isVoteFound = await service.IsVoteExistsAsync(fakeVoteId);

                //Assert
                Assert.False(isVoteFound);
            }
        }

        [Fact]
        public async Task TellIfVoteDoesNotExistByStringId()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                var isVoteFound = await service.IsVoteExistsAsync(fakeVoteId.ToString());

                //Assert
                Assert.False(isVoteFound);
            }
        }

        [Fact]
        public async Task Vote()
        {
            //Arrange
            var dbName = Guid.NewGuid().ToString();
            var fakeVoteId = Guid.NewGuid();
            var fakeVotedFor = "Messi";
            var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

            using (var context = GetContext(dbName))
            {
                //Act
                IVotingServices service = new VotingServices(context);
                await service.VoteAsync(fakeVote);
            }

            using (var context = GetContext(dbName))
            {
                var votesInDb = await context.Votes.CountAsync();
                var actualVote = await context.Votes.FirstAsync();

                //Assert
                Assert.Equal(1, votesInDb);
                Assert.Equal(fakeVote.VoterId, actualVote.VoterId);
                Assert.Equal(fakeVote.VotedFor, actualVote.VotedFor);
            }
        }

        private VoteContextDbApp GetContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<VoteContextDbApp>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new VoteContextDbApp(options);
            return context;
        }

        private List<Vote> GetVotes(int numberOfVotes)
        {
            var fakeVotes = new List<Vote>();

            for (int i = 0; i < numberOfVotes; i++)
            {
                var fakeVoteId = Guid.NewGuid();
                var choise = new Random().Next(0, 1);
                var fakeVotedFor = (choise == 0 ? "Messi" : "Ronaldo");
                var fakeVote = new Vote { VoterId = fakeVoteId, VotedFor = fakeVotedFor };

                fakeVotes.Add(fakeVote);
            }

            return fakeVotes;
        }
    }
}
