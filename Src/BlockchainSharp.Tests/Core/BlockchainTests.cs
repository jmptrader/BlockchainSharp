﻿namespace BlockchainSharp.Tests.Core
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BlockchainSharp.Core;

    [TestClass]
    public class BlockChainTests
    {
        [TestMethod]
        public void CreateWithInitialBlock()
        {
            Block block = new Block(0, null);
            BlockChain2 blockchain = new BlockChain2(block);

            Assert.AreEqual(0, blockchain.BestBlockNumber);
        }

        [TestMethod]
        public void RejectNonGenesisInitialBlock()
        {
            Block block = new Block(1, null);

            try
            {
                new BlockChain2(block);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
                Assert.AreEqual("Initial block should be genesis", ex.Message);
            }
        }

        [TestMethod]
        public void AddGenesisChildBlock()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, genesis.Hash);

            BlockChain2 chain = new BlockChain2(genesis);

            Assert.IsTrue(chain.TryToAdd(block));
            Assert.AreEqual(1, chain.BestBlockNumber);
        }

        [TestMethod]
        public void RejectBlockWithDifferentParent()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, new Hash());

            BlockChain2 chain = new BlockChain2(genesis);

            Assert.IsFalse(chain.TryToAdd(block));
            Assert.AreEqual(0, chain.BestBlockNumber);
        }

        [TestMethod]
        public void RejectBlockWithInvalidNumber()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(2, genesis.Hash);

            BlockChain2 chain = new BlockChain2(genesis);

            Assert.IsFalse(chain.TryToAdd(block));
            Assert.AreEqual(0, chain.BestBlockNumber);
        }

        [TestMethod]
        public void GetBlockByNumber()
        {
            IList<Block> blocks = new List<Block>();
            Block parent = null;
            BlockChain2 chain = null;

            for (int k = 0; k < 10; k++)
            {
                Block block = new Block(k, parent != null ? parent.Hash : null);
                blocks.Add(block);
                parent = block;

                if (chain == null)
                    chain = new BlockChain2(block);
                else
                    Assert.IsTrue(chain.TryToAdd(block));
            }

            Assert.AreEqual(9, chain.BestBlockNumber);

            for (int k = 0; k < 10; k++)
            {
                Block block = chain.GetBlock(k);

                Assert.IsNotNull(block);
                Assert.AreEqual(blocks[k], block);
                Assert.AreEqual(k, block.Number);
            }

            Assert.IsNull(chain.GetBlock(10));
            Assert.IsNull(chain.GetBlock(-1));
        }
    }
}

