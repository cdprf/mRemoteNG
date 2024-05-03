﻿using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Security;
using System.Xml.Linq;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Security;
using mRemoteNG.Tree;
using mRemoteNG.Tree.Root;

namespace mRemoteNG.Config.Serializers.ConnectionSerializers.Xml
{
    [SupportedOSPlatform("windows")]
    public class XmlConnectionsDocumentCompiler
    {
        private readonly ICryptographyProvider _cryptographyProvider;
        private SecureString _encryptionKey;
        private readonly ISerializer<ConnectionInfo, XElement> _connectionNodeSerializer;

        public XmlConnectionsDocumentCompiler(ICryptographyProvider cryptographyProvider, ISerializer<ConnectionInfo, XElement> connectionNodeSerializer)
        {
            _cryptographyProvider = cryptographyProvider ?? throw new ArgumentNullException(nameof(cryptographyProvider));
            _connectionNodeSerializer = connectionNodeSerializer ?? throw new ArgumentNullException(nameof(connectionNodeSerializer));
        }

        public XDocument CompileDocument(ConnectionTreeModel connectionTreeModel, bool fullFileEncryption)
        {
            RootNodeInfo rootNodeInfo = GetRootNodeFromConnectionTreeModel(connectionTreeModel);
            return CompileDocument(rootNodeInfo, fullFileEncryption);
        }

        public XDocument CompileDocument(ConnectionInfo serializationTarget, bool fullFileEncryption)
        {
            RootNodeInfo rootNodeInfo = GetRootNodeFromConnectionInfo(serializationTarget);
            _encryptionKey = rootNodeInfo.PasswordString.ConvertToSecureString();
            XElement rootElement = CompileRootNode(rootNodeInfo, fullFileEncryption);

            CompileRecursive(serializationTarget, rootElement);
            XDeclaration xmlDeclaration = new("1.0", "utf-8", null);
            XDocument xmlDocument = new(xmlDeclaration, rootElement);
            if (fullFileEncryption)
                xmlDocument = new XmlConnectionsDocumentEncryptor(_cryptographyProvider).EncryptDocument(xmlDocument, _encryptionKey);
            return xmlDocument;
        }

        private void CompileRecursive(ConnectionInfo serializationTarget, XContainer parentElement)
        {
            XContainer newElement = parentElement;
            if (serializationTarget is not RootNodeInfo)
            {
                newElement = CompileConnectionInfoNode(serializationTarget);
                parentElement.Add(newElement);
            }

            if (serializationTarget is not ContainerInfo serializationTargetAsContainer) return;
            foreach (ConnectionInfo child in serializationTargetAsContainer.Children.ToArray())
                CompileRecursive(child, newElement);
        }

        private static RootNodeInfo GetRootNodeFromConnectionTreeModel(ConnectionTreeModel connectionTreeModel)
        {
            return (RootNodeInfo)connectionTreeModel.RootNodes.First(node => node is RootNodeInfo);
        }

        private static RootNodeInfo GetRootNodeFromConnectionInfo(ConnectionInfo connectionInfo)
        {
            while (true)
            {
                if (connectionInfo is RootNodeInfo connectionInfoAsRootNode) return connectionInfoAsRootNode;
                connectionInfo = connectionInfo?.Parent ?? new RootNodeInfo(RootNodeType.Connection);
            }
        }

        private XElement CompileRootNode(RootNodeInfo rootNodeInfo, bool fullFileEncryption)
        {
            XmlRootNodeSerializer rootNodeSerializer = new();
            return rootNodeSerializer.SerializeRootNodeInfo(rootNodeInfo, _cryptographyProvider, _connectionNodeSerializer.Version, fullFileEncryption);
        }

        private XElement CompileConnectionInfoNode(ConnectionInfo connectionInfo)
        {
            return _connectionNodeSerializer.Serialize(connectionInfo);
        }
    }
}