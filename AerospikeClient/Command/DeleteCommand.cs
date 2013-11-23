/*
 * Aerospike Client - C# Library
 *
 * Copyright 2013 by Aerospike, Inc. All rights reserved.
 *
 * Availability of this source code to partners and customers includes
 * redistribution rights covered by individual contract. Please check your
 * contract for exact rights and responsibilities.
 */
namespace Aerospike.Client
{
	public sealed class DeleteCommand : SingleCommand
	{
		private readonly WritePolicy policy;
		private bool existed;

		public DeleteCommand(Cluster cluster, WritePolicy policy, Key key) 
			: base(cluster, key)
		{
			this.policy = (policy == null) ? new WritePolicy() : policy;
		}

		protected internal override Policy GetPolicy()
		{
			return policy;
		}

		protected internal override void WriteBuffer()
		{
			SetDelete(policy, key);
		}

		protected internal override void ParseResult(Connection conn)
		{
			// Read header.
			conn.ReadFully(dataBuffer, MSG_TOTAL_HEADER_SIZE);

			int resultCode = dataBuffer[13];

			if (resultCode != 0 && resultCode != ResultCode.KEY_NOT_FOUND_ERROR)
			{
				throw new AerospikeException(resultCode);
			}
			existed = resultCode == 0;
			EmptySocket(conn);
		}

		public bool Existed()
		{
			return existed;
		}
	}
}