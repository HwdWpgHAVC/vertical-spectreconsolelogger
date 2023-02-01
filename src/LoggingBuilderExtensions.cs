using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Vertical.SpectreLogger.Core;
using Vertical.SpectreLogger.Internal;
using Vertical.SpectreLogger.Options;
using Vertical.SpectreLogger.Output;
using Vertical.SpectreLogger.Rendering;
using Vertical.SpectreLogger.Scopes;
using Vertical.SpectreLogger.Templates;

namespace Vertical.SpectreLogger
{
    /// <summary>
    /// Extensions for <see cref="ILoggingBuilder"/>
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Adds the Spectre console logging provider.
        /// </summary>
        /// <param name="builder">Logging builder instance</param>
        /// <param name="configureBuilder">A delegate that receives an options that can control
        /// how the logger renders events.</param>
        /// <returns><paramref name="builder"/></returns>
        public static ILoggingBuilder AddSpectreConsole(
            this ILoggingBuilder builder,
            Action<SpectreLoggingBuilder>? configureBuilder = null)
        {
            var services = builder.Services;
            var optionsBuilder = new SpectreLoggingBuilder(services);
            
            services.AddTransient<ITemplateRendererBuilder, TemplateRendererBuilder>();
            services.AddSingleton(AnsiConsole.Console);
            services.AddSingleton<ScopeManager>();
            services.AddSingleton<IRendererPipeline, RendererPipeline>();
            services.AddSingleton<ILoggerProvider, SpectreLoggerProvider>();
            services.AddTransient<IWriteBuffer, WriteBuffer>();
            services.AddSingleton<ObjectPool<IWriteBuffer>>(sp => new DefaultObjectPool<IWriteBuffer>(
                new WriteBufferPooledObjectPolicy(
                    sp.GetRequiredService<IConsoleWriter>(),
                    sp.GetRequiredService<IWriteBuffer>),
                sp.GetRequiredService<IOptions<SpectreLoggerOptions>>().Value.MaxPooledBuffers));

            optionsBuilder
                .AddTemplateRenderers()
                .WriteInForeground()
                .UseConsole(AnsiConsole.Console)
                .SetMinimumLevel(LogLevel.Information);

            configureBuilder?.Invoke(optionsBuilder);

            return builder;
        }

        /// <summary>
        /// Adds SpectreConsole logging uses <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="configureBuilder">Action with parameter <see cref="SpectreLoggingBuilder"/></param>
        /// <returns></returns>
        public static IServiceCollection AddSpectreConsoleLogging(this IServiceCollection services, Action<SpectreLoggingBuilder>? configureBuilder = null)
        {

            var optionsBuilder = new SpectreLoggingBuilder(services);

            services.AddTransient<ITemplateRendererBuilder, TemplateRendererBuilder>();
            services.AddSingleton(AnsiConsole.Console);
            services.AddSingleton<ScopeManager>();
            services.AddSingleton<IRendererPipeline, RendererPipeline>();
            services.AddSingleton<ILoggerProvider, SpectreLoggerProvider>();
            services.AddTransient<IWriteBuffer, WriteBuffer>();
            services.AddSingleton<ObjectPool<IWriteBuffer>>(sp => new DefaultObjectPool<IWriteBuffer>(
                new WriteBufferPooledObjectPolicy(
                    sp.GetRequiredService<IConsoleWriter>(),
                    sp.GetRequiredService<IWriteBuffer>),
                sp.GetRequiredService<IOptions<SpectreLoggerOptions>>().Value.MaxPooledBuffers));

            optionsBuilder
                .AddTemplateRenderers()
                .WriteInForeground()
                .UseConsole(AnsiConsole.Console)
                .SetMinimumLevel(LogLevel.Information);

            configureBuilder?.Invoke(optionsBuilder);


            return services;
        }
    }
}