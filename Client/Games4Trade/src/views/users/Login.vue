<template>
    <div>
    <div id="login-form" class="row no-gutters">
        <div class="form rounded col-4 offset-4 p-4">
            <form @submit.prevent="onSubmit">
                <div class="form-group">
                    <label for="login">Login</label>
                    <input
                            type="text"
                            id="login"
                            class="form-control"
                            v-model="user.login">
                </div>
                <div class="form-group">
                    <label for="password">Hasło</label>
                    <input
                            type="password"
                            id="password"
                            class="form-control"
                            v-model="user.password">
                </div>
                <p style="color: red" v-if="wrongPassword">Podano nie prawidłową kombinację loginu i hasła.</p>
                <div class="form-group d-flex justify-content-between">
                    <span class="small">
                        <a class="page-link" @click="onRecover">Nie pamiętasz hasła ?</a>
                    </span>
                    <div class="submit">
                        <button type="submit" class="btn btn-info">Zaloguj</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
    </div>
</template>

<script>
import axios from 'axios'
export default {
  name: 'Login',
  data () {
    return {
      user: {
        login: '',
        password: ''
      },
      wrongPassword: false
    }
  },
  methods: {
    onSubmit () {
      this.$store.dispatch('setSpinnerLoading')
      this.$store.dispatch('login', this.user)
        .then(() => {
          this.$store.dispatch('unsetSpinnerLoading')
          this.$router.push('/')
        })
        .catch(error => {
          this.$store.dispatch('unsetSpinnerLoading')
          if (error.response.status === 400) {
            this.wrongPassword = true
          } else {
            console.log(error)
          }
        })
    },
    async onRecover () {
      const {value: email} = await this.$swal({
        title: 'Przypominanie hasła',
        text: 'Proszę podać swój adres e-mail, zostanie na niego wysłany link do procedury przypominania hasła.',
        showCancelButton: true,
        confirmButtonText: 'Wyślij wiadomość',
        cancelButtonText: 'Nie wysyłaj',
        input: 'email'
      })
      if (email) {
        this.$store.dispatch('setSpinnerLoading')
        axios.post(`login/password/recover?email=${email}`)
          .then(() => {
            this.$store.dispatch('unsetSpinnerLoading')
            this.$swal({
              title: 'Wiadomość została wysłana',
              type: 'success'
            })
          })
          .catch(() => {
            this.$store.dispatch('unsetSpinnerLoading')
            this.$swal({
              title: 'Wystąpił nieoczekiwany błąd',
              text: 'Może wprowadziłes nie prawidłowy adres email ?',
              type: 'error'
            })
          })
      }
    }
  }
}
</script>
<style scoped>
</style>
