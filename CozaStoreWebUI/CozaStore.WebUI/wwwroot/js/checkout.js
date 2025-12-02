// Checkout sayfası için JavaScript kodları

document.addEventListener('DOMContentLoaded', function() {
    // Anti-forgery token'ı al
    const getToken = () => document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    // ============================================
    // Adres Kartı Seçim İşlemleri
    // ============================================
    
    // Shipping address seçimi
    const shippingCards = document.querySelectorAll('#shippingAddressCards .address-card:not(.add-address-card)');
    const shippingAddressInput = document.getElementById('shippingAddressId');
    
    shippingCards.forEach(card => {
        card.addEventListener('click', function() {
            // Diğer kartların seçimini kaldır
            shippingCards.forEach(c => c.classList.remove('selected'));
            // Bu kartı seç
            this.classList.add('selected');
            // Hidden input'a ID'yi yaz
            if (shippingAddressInput) {
                shippingAddressInput.value = this.dataset.addressId;
            }
        });
    });

    // Billing address seçimi
    const billingCards = document.querySelectorAll('#billingAddressCards .address-card');
    const billingAddressInput = document.getElementById('billingAddressId');
    
    billingCards.forEach(card => {
        card.addEventListener('click', function() {
            // Eğer zaten seçiliyse, seçimi kaldır (optional olduğu için)
            if (this.classList.contains('selected')) {
                this.classList.remove('selected');
                if (billingAddressInput) {
                    billingAddressInput.value = '';
                }
            } else {
                // Diğer kartların seçimini kaldır
                billingCards.forEach(c => c.classList.remove('selected'));
                // Bu kartı seç
                this.classList.add('selected');
                // Hidden input'a ID'yi yaz
                if (billingAddressInput) {
                    billingAddressInput.value = this.dataset.addressId;
                }
            }
        });
    });

    // ============================================
    // Yeni Adres Ekleme
    // ============================================
    
    const showFormBtn = document.getElementById('showAddressFormBtn');
    const newAddressForm = document.getElementById('newAddressForm');
    const cancelAddressBtn = document.getElementById('cancelAddressBtn');
    const saveAddressBtn = document.getElementById('saveAddressBtn');
    
    if (showFormBtn) {
        showFormBtn.addEventListener('click', function() {
            newAddressForm.classList.remove('hidden');
            newAddressForm.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    }
    
    if (cancelAddressBtn) {
        cancelAddressBtn.addEventListener('click', function() {
            // Eğer kayıtlı adres varsa formu gizle, yoksa gizleme (zorunlu)
            const hasAddresses = document.querySelectorAll('#shippingAddressCards .address-card:not(.add-address-card)').length > 0;
            if (hasAddresses) {
                newAddressForm.classList.add('hidden');
            }
            // Form alanlarını temizle
            document.getElementById('addressTitle').value = '';
            document.getElementById('addressLine1').value = '';
            document.getElementById('addressLine2').value = '';
            document.getElementById('city').value = '';
            document.getElementById('district').value = '';
            document.getElementById('postalCode').value = '';
            document.getElementById('country').value = 'Turkey';
        });
    }
    
    if (saveAddressBtn) {
        saveAddressBtn.addEventListener('click', async function() {
            const title = document.getElementById('addressTitle').value.trim();
            const line1 = document.getElementById('addressLine1').value.trim();
            const line2 = document.getElementById('addressLine2').value.trim();
            const city = document.getElementById('city').value.trim();
            const district = document.getElementById('district').value.trim();
            const postalCode = document.getElementById('postalCode').value.trim();
            const country = document.getElementById('country').value.trim() || 'Turkey';

            if (!title || !line1 || !city || !district) {
                alert('Lütfen tüm zorunlu alanları doldurun (Title, Address Line 1, City, District).');
                return;
            }

            // Butonu devre dışı bırak
            saveAddressBtn.disabled = true;
            const originalText = saveAddressBtn.innerHTML;
            saveAddressBtn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Kaydediliyor...';

            try {
                const formData = new FormData();
                formData.append('title', title);
                formData.append('addressLine1', line1);
                if (line2) formData.append('addressLine2', line2);
                formData.append('city', city);
                formData.append('district', district);
                if (postalCode) formData.append('postalCode', postalCode);
                formData.append('country', country);
                const token = getToken();
                if (token) formData.append('__RequestVerificationToken', token);

                const response = await fetch('/Checkout/SaveAddress', {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    // Sayfa yenilenmeden önce kısa bekleme
                    setTimeout(() => {
                        window.location.reload();
                    }, 500);
                } else {
                    alert(result.message || 'Adres kaydedilemedi. Lütfen tekrar deneyin.');
                }
            } catch (error) {
                console.error('Error saving address:', error);
                alert('Bir hata oluştu. Lütfen tekrar deneyin.');
            } finally {
                saveAddressBtn.disabled = false;
                saveAddressBtn.innerHTML = originalText;
            }
        });
    }

    // ============================================
    // Adres Düzenleme
    // ============================================
    
    const editModal = document.getElementById('editAddressModal');
    const closeEditModal = document.getElementById('closeEditModal');
    const cancelEditBtn = document.getElementById('cancelEditBtn');
    const saveEditBtn = document.getElementById('saveEditBtn');
    
    // Edit butonlarına event listener ekle
    document.querySelectorAll('.edit-address-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.stopPropagation(); // Kartın click event'ini tetikleme
            
            // Modal'a veriyi yükle
            document.getElementById('editAddressId').value = this.dataset.addressId;
            document.getElementById('editAddressTitle').value = this.dataset.title || '';
            document.getElementById('editAddressLine1').value = this.dataset.line1 || '';
            document.getElementById('editAddressLine2').value = this.dataset.line2 || '';
            document.getElementById('editCity').value = this.dataset.city || '';
            document.getElementById('editDistrict').value = this.dataset.district || '';
            document.getElementById('editPostalCode').value = this.dataset.postal || '';
            document.getElementById('editCountry').value = this.dataset.country || 'Turkey';
            
            // Modal'ı göster
            editModal.classList.add('show');
        });
    });
    
    // Modal'ı kapat
    const closeModal = () => {
        editModal.classList.remove('show');
    };
    
    if (closeEditModal) {
        closeEditModal.addEventListener('click', closeModal);
    }
    
    if (cancelEditBtn) {
        cancelEditBtn.addEventListener('click', closeModal);
    }
    
    // Modal dışına tıklanırsa kapat
    editModal?.addEventListener('click', function(e) {
        if (e.target === editModal) {
            closeModal();
        }
    });
    
    // Adres güncelleme
    if (saveEditBtn) {
        saveEditBtn.addEventListener('click', async function() {
            const id = document.getElementById('editAddressId').value;
            const title = document.getElementById('editAddressTitle').value.trim();
            const line1 = document.getElementById('editAddressLine1').value.trim();
            const line2 = document.getElementById('editAddressLine2').value.trim();
            const city = document.getElementById('editCity').value.trim();
            const district = document.getElementById('editDistrict').value.trim();
            const postalCode = document.getElementById('editPostalCode').value.trim();
            const country = document.getElementById('editCountry').value.trim() || 'Turkey';

            if (!title || !line1 || !city || !district) {
                alert('Lütfen tüm zorunlu alanları doldurun.');
                return;
            }

            saveEditBtn.disabled = true;
            const originalText = saveEditBtn.innerHTML;
            saveEditBtn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Güncelleniyor...';

            try {
                const formData = new FormData();
                formData.append('id', id);
                formData.append('title', title);
                formData.append('addressLine1', line1);
                if (line2) formData.append('addressLine2', line2);
                formData.append('city', city);
                formData.append('district', district);
                if (postalCode) formData.append('postalCode', postalCode);
                formData.append('country', country);
                const token = getToken();
                if (token) formData.append('__RequestVerificationToken', token);

                const response = await fetch('/Checkout/UpdateAddress', {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    alert('Adres başarıyla güncellendi! Sayfa yenilenecek.');
                    window.location.reload();
                } else {
                    alert(result.message || 'Adres güncellenemedi.');
                }
            } catch (error) {
                console.error('Error updating address:', error);
                alert('Bir hata oluştu. Lütfen tekrar deneyin.');
            } finally {
                saveEditBtn.disabled = false;
                saveEditBtn.innerHTML = originalText;
            }
        });
    }

    // ============================================
    // Adres Silme
    // ============================================
    
    document.querySelectorAll('.delete-address-btn').forEach(btn => {
        btn.addEventListener('click', async function(e) {
            e.stopPropagation(); // Kartın click event'ini tetikleme
            
            const addressId = this.dataset.addressId;
            const addressTitle = this.dataset.title;
            
            if (!confirm(`"${addressTitle}" adresini silmek istediğinizden emin misiniz?`)) {
                return;
            }

            const card = this.closest('.address-card');
            card.classList.add('loading');

            try {
                const formData = new FormData();
                formData.append('id', addressId);
                const token = getToken();
                if (token) formData.append('__RequestVerificationToken', token);

                const response = await fetch('/Checkout/DeleteAddress', {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    alert('Adres başarıyla silindi! Sayfa yenilenecek.');
                    window.location.reload();
                } else {
                    alert(result.message || 'Adres silinemedi.');
                    card.classList.remove('loading');
                }
            } catch (error) {
                console.error('Error deleting address:', error);
                alert('Bir hata oluştu. Lütfen tekrar deneyin.');
                card.classList.remove('loading');
            }
        });
    });

    // ============================================
    // Form Submit Validasyonu
    // ============================================
    
    const checkoutForm = document.getElementById('checkoutForm');
    
    if (checkoutForm) {
        checkoutForm.addEventListener('submit', function(e) {
            const shippingAddressId = document.getElementById('shippingAddressId')?.value;
            
            if (!shippingAddressId || shippingAddressId === '') {
                e.preventDefault();
                alert('Lütfen teslimat adresi seçin.');
                return false;
            }
            
            // Place Order butonunu devre dışı bırak
            const submitBtn = checkoutForm.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Sipariş oluşturuluyor...';
            }
        });
    }
});

