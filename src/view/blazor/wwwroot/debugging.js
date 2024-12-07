import { dotnet } from './dotnet.js'
await dotnet
    .withDiagnosticTracing(true) // enable JavaScript tracing
    .withConfig({
        environmentVariables: {
            "MONO_LOG_LEVEL": "debug", //enable Mono VM detailed logging by
            "MONO_LOG_MASK": "all", // categories, could be also gc,aot,type,...
        }
    })
    .run();