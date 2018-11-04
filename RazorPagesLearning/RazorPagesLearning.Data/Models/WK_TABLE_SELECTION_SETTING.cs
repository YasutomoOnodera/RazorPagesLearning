using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �e�[�u����̃`�F�b�N�{�b�N�X�I����ԊǗ�
    /// </summary>
    [Serializable]
    public class WK_TABLE_SELECTION_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
        public WK_TABLE_SELECTION_SETTING()
        {
        }

        /// <summary>
        /// ID
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
        /// ���f�[�^��ID
        /// </summary>
        public Int64 originalDataId { get; set; }

        /// <summary>
        /// �v�f��I���ς݂�
        /// </summary>
        public bool selected { get; set; }

        /// <summary>
        /// �t�����
        /// �K�v�ɉ����Ďg�p
        /// </summary>
        public string appendInfo { get; set; }

    }
}