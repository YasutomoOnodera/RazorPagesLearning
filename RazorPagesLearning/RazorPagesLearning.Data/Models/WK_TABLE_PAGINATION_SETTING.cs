using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �e�[�u���y�[�W�l�[�V�����ݒ���
    /// </summary>
    [Serializable]
    public class WK_TABLE_PAGINATION_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
        public WK_TABLE_PAGINATION_SETTING()
        {
        }

        /// <summary>
        ///ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ID { get; set; }

        /// <summary>
        /// �֘A�t�����ꂽ���[�U�[�A�J�E���gID
        /// </summary>
        public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// �ΏۂƂȂ�e�[�u��
        /// </summary>
        public ViewTableType viewTableType { get; set; }

        /// <summary>
        /// �S�v�f��I���ς݂�
        /// </summary>
        public bool checkAllPage { get; set; }

    }
}