# aura-search-net
Lightweight search library for .NET 

It's based on dynamic weights. 

Quick start:  

Register Dependencies: (with your implementation for repositories) 


           // register services
            services.AddTransient<IWordRepository, WordRepository>();
            services.AddTransient<IThoughtRepository, ThoughtRepository>();
            services.AddTransient<ISymbolComparer, DefaultSymbolComparer>();
            services.AddTransient<IStringCompareAlgorithm, StringMatcher>(); 
            
            
   services.AddTransient<IContextRepository, ClientRepository>(); 
   
   
    //register usecases  
            services.AddTransient<IUseCase<StoreIndexRequest>, CreateIndexUseCase>(); 
            services.AddTransient<IUseCase<ReinforceRequest>, ReinforceUseCase>(); 
            services.AddTransient<IUseCase<SearchRequest>, SearchUseCase>(); 
            services.AddSingleton<IOptions<Settings>>(_ => Options.Create<Settings>(new Settings()));

            // register filters  
            services.AddTransient<IInputFilter<ReinforceRequest>, ReinforceTokenizerFilter>();
            services.AddTransient<IInputFilter<StoreIndexRequest>, TokenizerInputFilter>();
            services.AddTransient<IInputFilter<SearchRequest>, TokenizerSearchRequestFilter>();
            services.AddTransient<IInputFilter<StoreIndexRequest>, ProduceSymbolsFilter>();

            //// Register event handlers 
            services.AddTransient(typeof(IEventHandler<ActivationEvent>), typeof(MatchMetricHandler));
            services.AddTransient(typeof(IEventHandler<MistmatchEvent>), typeof(MismatchMetricHandler));
            services.AddTransient(typeof(IEventHandler<RefreshEvent>), typeof(RefreshEventHandler));
            services.AddTransient(typeof(IEventHandler<AddToContextEvent>), typeof(AddToContextHandler));
            
            
# How TO USE


# Create an Index 

    var context = await _contextAccessor.GetContextAsync(request.ClientIdentifier, cancellationToken);
            var storeRequest = new StoreIndexRequest
            {
                ClientIdentifier = context.ClientIdentifier,
                Payloads = new List<Payload> {
                     new Payload
                     {
                          Document = request.OriginalRequest,
                          Title = request.OriginalRequest
                     }
                 }
            };

            await _storeUseCase.Execute(storeRequest, context, cancellationToken);
            
 # Search 

        var context = await _contextAccessor.GetContextAsync(request.ClientIdentifier, cancellationToken);

            var searchRequest = new SearchRequest
            {
                ClientIdentifier = context.ClientIdentifier,
                Request = request.OriginalRequest
            };

            await _searchUseCase.Execute(searchRequest, context, cancellationToken);
 

            var lastResult = _searchOutput.GetLatestResult(request.ClientIdentifier); // The search results should be accessed from output port that has been registered in services  
            
 # Reinforce 
 
 
  var context = await _contextAccessor.GetContextAsync(request.ClientIdentifier, cancellationToken);

                var reinforceRequest = new ReinforceRequest
                {
                    ClientIdentifier = request.ClientIdentifier,
                    Type = ReinforceType.Boost,
                    ThoughtId = request.Context.State.ThoughtId,
                    IdeaId = request.Context.State.IdeaId,
                    OriginalRequest = request.Context.State.Request
                };

                await _reinforceUseCase.Execute(reinforceRequest, context, cancellationToken);
                
                
                
  # Append Index  
  
     var context = await _contextAccessor.GetContextAsync(request.ClientIdentifier, cancellationToken);

            var reinforceRequest = new ReinforceRequest
            {
                ClientIdentifier = request.ClientIdentifier,
                Type = ReinforceType.Append,
                ThoughtId = request.Context.State.ThoughtId,
                Addition = request.OriginalRequest,
                OriginalRequest = request.Context.State.Request
            };

            await _reinforceUseCase.Execute(reinforceRequest, context, cancellationToken);


# Settings 

Also you can adjust settings related to indexing, search thresshold 

```
   public sealed class Settings
    {
        public double DefNeuronDom { get; set; } = 0.02d;
        public double NeuronReinforce { get; set; } = 0.0030d;
        public int MinimumSymbolLength { get; set; } = 2;
        public double NeuronDominanceWeigh { get; set; } = 0.08d;
        public double ActivationThress { get; set; } = .58d;
      
        public int WordCounterThresshold = 3;

        public int WordsToIndexThresshold = 15; 
    } 
```
            
