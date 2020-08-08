# TweekBook

This is a sample application which saves the posts . But the basic idea behind this repository is to transfer the knowledge about building blocks used for a real time project : 

1 . Swagger Implemnatation 
    
    Install package "Swashbuckle.AspNetCore" . This package installs Swagger Gen and Swagger UI beneath it . 
    
    In the startup while
    configuring the services we use AddSwaggerGen() method and customise it 
    In the middleware we need to implement UseSwagger() and UseSwaggerUI() to enable UI and to generate end points .
    
 2 . Implementing Versioning 
  
    API versioning help you in maintanince of code . API version can be driven from the controller and even on the contracts. 
   
 3. Setting Up JWT Support 
      
      First thing is to have a secret key for your app . In this piece of code you can find it in appsettings .(But for a real time project it shouldn't be shared) 
      
      Configure the service with AddAuthentication() using the defaultAuthenticationscheme,DefaultScheme,DefaultChallengeScheme to JWTBearerDefaults.AuthenticationSchema
      
      and user AddJwtBearer() method to tell system about what the token should look like and its rules using the TokenValidationParameters class. The proper IssuersignInKey
      is the mapped with the secret key of the application using the SymetricSecurityKey . And there are many other things we can configure apart from the properties defined in 
      the code .
      
      As swagger gen also need to know about this , we need to configure our swagger to support bearer token . We use AddSecuityDefinition() and AddSecurityRequirements()
      methods to enable authentication for our swagger application . 
      
      
      In the middlewear we need add UseAuthentication().
      
  4. Setting Up API Key based Authentication 
    
        When we need to expose our api's as external service , this way of authentication helps the application .Key can be in the header , query as parameter but here we are
        doing it in header our example . 
        
        We created this as an attribute and extended the IAsyncActionFilter interface . This is written as a middlewear.
 
      
      
