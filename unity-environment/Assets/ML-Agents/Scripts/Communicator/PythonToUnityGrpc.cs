// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: communicator/python_to_unity.proto
// </auto-generated>
#pragma warning disable 1591
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace MLAgents.Communicator {
  /// <summary>
  /// This would require two separate channels ?
  /// </summary>
  public static partial class PythonToUnity
  {
    static readonly string __ServiceName = "communicator.PythonToUnity";

    static readonly grpc::Marshaller<global::MLAgents.Communicator.PythonParameters> __Marshaller_PythonParameters = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::MLAgents.Communicator.PythonParameters.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::MLAgents.Communicator.AcademyParameters> __Marshaller_AcademyParameters = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::MLAgents.Communicator.AcademyParameters.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::MLAgents.Communicator.UnityInput> __Marshaller_UnityInput = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::MLAgents.Communicator.UnityInput.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::MLAgents.Communicator.UnityOutput> __Marshaller_UnityOutput = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::MLAgents.Communicator.UnityOutput.Parser.ParseFrom);

    static readonly grpc::Method<global::MLAgents.Communicator.PythonParameters, global::MLAgents.Communicator.AcademyParameters> __Method_Initialize = new grpc::Method<global::MLAgents.Communicator.PythonParameters, global::MLAgents.Communicator.AcademyParameters>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Initialize",
        __Marshaller_PythonParameters,
        __Marshaller_AcademyParameters);

    static readonly grpc::Method<global::MLAgents.Communicator.UnityInput, global::MLAgents.Communicator.UnityOutput> __Method_Send = new grpc::Method<global::MLAgents.Communicator.UnityInput, global::MLAgents.Communicator.UnityOutput>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Send",
        __Marshaller_UnityInput,
        __Marshaller_UnityOutput);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::MLAgents.Communicator.PythonToUnityReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of PythonToUnity</summary>
    public abstract partial class PythonToUnityBase
    {
      /// <summary>
      /// Sends the academy parameters
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      public virtual global::System.Threading.Tasks.Task<global::MLAgents.Communicator.AcademyParameters> Initialize(global::MLAgents.Communicator.PythonParameters request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::MLAgents.Communicator.UnityOutput> Send(global::MLAgents.Communicator.UnityInput request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for PythonToUnity</summary>
    public partial class PythonToUnityClient : grpc::ClientBase<PythonToUnityClient>
    {
      /// <summary>Creates a new client for PythonToUnity</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public PythonToUnityClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for PythonToUnity that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public PythonToUnityClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected PythonToUnityClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected PythonToUnityClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Sends the academy parameters
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::MLAgents.Communicator.AcademyParameters Initialize(global::MLAgents.Communicator.PythonParameters request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Initialize(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Sends the academy parameters
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      public virtual global::MLAgents.Communicator.AcademyParameters Initialize(global::MLAgents.Communicator.PythonParameters request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Initialize, null, options, request);
      }
      /// <summary>
      /// Sends the academy parameters
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::MLAgents.Communicator.AcademyParameters> InitializeAsync(global::MLAgents.Communicator.PythonParameters request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return InitializeAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Sends the academy parameters
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncUnaryCall<global::MLAgents.Communicator.AcademyParameters> InitializeAsync(global::MLAgents.Communicator.PythonParameters request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Initialize, null, options, request);
      }
      public virtual global::MLAgents.Communicator.UnityOutput Send(global::MLAgents.Communicator.UnityInput request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return Send(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::MLAgents.Communicator.UnityOutput Send(global::MLAgents.Communicator.UnityInput request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Send, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::MLAgents.Communicator.UnityOutput> SendAsync(global::MLAgents.Communicator.UnityInput request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return SendAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::MLAgents.Communicator.UnityOutput> SendAsync(global::MLAgents.Communicator.UnityInput request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Send, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override PythonToUnityClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new PythonToUnityClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(PythonToUnityBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Initialize, serviceImpl.Initialize)
          .AddMethod(__Method_Send, serviceImpl.Send).Build();
    }

  }
}
#endregion