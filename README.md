# DeathRaffle

Death befalls even the famous.  You can bet on it.

A local group of random strangers gather together at a local bar and participate in what they call a 'Death Party'.  A small tub holding strips of paper of which each contain the name, age, and short description of a world famous celebrity.

The visitor randomly selects one of the slips and pays a small fee, usually $20 USD into the pool.  That participants name, email address, and celebrity are written into a log.  From there, the participant can enjoy all the pizza and beer they wish provided by the host.

So who's the host?  Well, the person who won the pot since the last party of course!  The objective of the game is to be the one holding the celebrity who passes away first in real life.  At that point, the game organizer will email the winner to let them know they've won and it will then be the winner's responsibility to schedule and host the next party and provide the beer and pizza.

Generally the game pool is just enough money to cover the cost of hosting the party and a few extra bucks for the winner.

So what is DeathRaffle?

DeathRaffle is an online version of the same game but on a much larger scale.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them

```
- [.Net Core 2.0](https://github.com/dotnet/core)
- [Supported Operating Systems](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md)
- JetBrains Rider, VS Code, or Visual Studio
```

### Installing

```
- Clone this repository.
- Run dotnet build via commandline or using your IDE of choice.
- Run npm run serve to try it out locally or npm run build to make a production build of the Vue application.
```

## Running the tests

There are no tests on either of the server-side or client-side applications.  Feel free to pitch in!

## Deployment

Once you've run the build/deploy for the server-side just copy the release/publish directory for the server-side and the /dist for the client-side to the appropriate services on both the cronjob project, api project, and Vue project.

## Contributing

To start with, I don't have time to put together a complete contributing guide.  So, if your so inclined, please submit a pull request and if there are any major issues I'll let you know or merge.  Thanks for pitching in!

## Versioning

We use [SemVer](http://semver.org/) for versioning. But this is the first release and the versions haven't yet been completely defined as of yet.

## Authors

**Joshua Andrews** - *Initial work* - [LuckyURE](https://github.com/LuckyURE)

## License
This work is licensed under a [Creative Commons Attribution-NonCommercial 3.0 Unported License](http://creativecommons.org/licenses/by-nc/3.0/).

## Acknowledgments

**Stephen Thompson** - *SQL Assistance*
