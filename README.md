# OpenTracing

To run:

1. Open terminal in the solution folder.
2. Run `docker-compose up -d` to start Jaeger.
3. Run the app, access Swagger UI, and execute the GET method multiple times (ProbabilisticSampler(0.1) is used to send only 10% of all traces)
4. Visit http://localhost:16686, select `open-tracing-service` as the Service, click FindTraces, open the WeatherForecast trace, span, and tags to view the custom forecastsCount tag.
